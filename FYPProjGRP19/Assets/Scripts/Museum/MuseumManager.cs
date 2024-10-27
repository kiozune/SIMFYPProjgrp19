using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CardCategory {
    [Tooltip("Name of the card category, i.e. \'1300s - Old Fishing Village\'")] 
        public string catName;
    [Tooltip("Card background")] public Material cardMaterial; 
}

public class MuseumManager : MonoBehaviour
{
    [Header("Museum Card Pages")] 
    [SerializeField] private GameObject prevPage;
    [SerializeField] private GameObject currentPage;
    [SerializeField] private GameObject nextPage;
    [SerializeField,
        Tooltip("Name of card category currently in view")]
        private TMP_Text currentCatName; 

    [Header("Card Category Tracking")]
    [SerializeField] private CardCategory[] cardCategories;
    [SerializeField, Tooltip("Number of unique cards per level")] private int numOfCards = 6;
    [SerializeField] private GameObject[] prevCards;
    [SerializeField] private GameObject[] currentCards;
    [SerializeField] private GameObject[] nextCards;
    private int cardIdx = 0;

    [Header("Movement Anchors")]
    [SerializeField] private GameObject leftAnchor;
    [SerializeField] private GameObject centerAnchor;
    [SerializeField] private GameObject rightAnchor;

    [Header("Bool checks")]
    private bool isTurningPage = false;

    void Start()
    {
        // pages
        if (prevPage == null)
        {
            prevPage = GameObject.Find("Prev Page");
            if (prevPage == null) Debug.LogError("\"Prev Page\" could not be found"); 
        }
        if (currentPage == null)
        {
            currentPage = GameObject.Find("Current Page");
            if (currentPage == null) Debug.LogError("\"Current Page\" could not be found");
        }
        if (nextPage == null)
        {
            nextPage = GameObject.Find("Next Page");
            if (nextPage == null) Debug.LogError("\"Next Page\" could not be found"); 
        }

        // category name text
        if (currentCatName == null)
        {
            currentCatName = GameObject.Find("Card Category Txt").GetComponent<TMP_Text>();
            if (currentCatName == null) Debug.LogError("\"Card Category Txt\" could not be found");
        }

        // card arrays
        if (cardCategories.Length == 0) Debug.LogError("No card categories have been created");
        if (prevCards.Length == 0) Debug.LogError("No prev page cards have been assigned");
        if (currentCards.Length == 0) Debug.LogError("No current page cards have been assigned");
        if (nextCards.Length == 0) Debug.LogError("No next page cards have been assigned");

        // view anchors
        if (leftAnchor == null)
        {
            leftAnchor = GameObject.Find("Left Card Group Anchor");
            if (leftAnchor == null) Debug.LogError("Left anchor could not be found");
        }
        if (centerAnchor == null)
        {
            centerAnchor = GameObject.Find("Center Card Group Anchor");
            if (centerAnchor == null) Debug.LogError("Center anchor could not be found");
        }
        if (rightAnchor == null)
        {
            rightAnchor = GameObject.Find("Right Card Group Anchor");
            if (rightAnchor == null) Debug.LogError("Right anchor could not be found");
        }
    }

    private void ChangeCardMat(GameObject[] cardArr, Material cardMat)
    {
        foreach (GameObject card in cardArr)
        {
            Renderer r = card.GetComponent<Renderer>();
            if (r == null)
            {
                Debug.LogError(card.name + " does not have a valid renderer, skipped");
                continue;
            }
            else r.material = cardMat;
        }
    }

    private IEnumerator FlipPage(GameObject cardGrp, GameObject[] cardArr, string triggerName, GameObject anchor)
    {
        foreach (GameObject card in cardArr)
        {
            Animator a = card.GetComponent<Animator>();
            if (a == null)
            {
                Debug.LogError(card.name + " does not have a valid animator, skipped");
                continue;
            }
            else a.SetTrigger(triggerName);
        }

        if (anchor.name != "Left Card Group Anchor" && anchor.name != "Right Card Group Anchor")
        {
            ViewCard[] viewCardScripts = cardGrp.GetComponentsInChildren<ViewCard>();
            foreach (ViewCard viewCard in viewCardScripts)
                viewCard.UpdateCard(cardIdx);
        }

        float animLength = 0.065f; //GetCurrentClipInfo(0).Length not returning expected value
        Vector3 startPos = cardGrp.transform.position; 
        Vector3 endPos = anchor.transform.position;
        float elapsedTime = 0;
        while (elapsedTime < animLength)
        {
            cardGrp.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / animLength);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // change the material of the center card group when off-screen
        // for the illusion of swiping pages
        if (anchor.name == "Left Card Group Anchor" || anchor.name == "Right Card Group Anchor")
        {
            ChangeCardMat(cardArr, cardCategories[cardIdx].cardMaterial);
            ViewCard[] viewCardScripts = cardGrp.GetComponentsInChildren<ViewCard>();
            foreach (ViewCard viewCard in viewCardScripts)
                viewCard.UpdateCard(cardIdx);
        }
        // reset positions
        cardGrp.transform.position = startPos;

        isTurningPage = false;
    }

    private void UpdateCatName()
    {
        currentCatName.SetText(cardCategories[cardIdx].catName);
    }

    /// <summary>
    /// To be called by the Next Page button
    /// </summary>
    public void NextPage()
    {
        // Debug.Log("Next Page button has been clicked!");
        if (!isTurningPage)
        {
            isTurningPage = true; // prevent repeat animations 

            cardIdx++;
            if (cardIdx == cardCategories.Length) cardIdx = 0; // bring back to starting index
             
            ChangeCardMat(nextCards, cardCategories[cardIdx].cardMaterial);
            StartCoroutine(FlipPage(currentPage, currentCards, "Next Page", leftAnchor));
            StartCoroutine(FlipPage(nextPage, nextCards, "Next Page", centerAnchor));

            UpdateCatName();
        }
    } 

    /// <summary>
    /// To be called by the Prev Page button
    /// </summary>
    public void PrevPage()
    {
        // Debug.Log("Prev Page button has been clicked!");
        if (!isTurningPage)
        {
            isTurningPage = true; // prevent repeat animations
            prevPage.SetActive(true);

            cardIdx--;
            if (cardIdx == -1) cardIdx = cardCategories.Length - 1; // bring back to last index

            ChangeCardMat(prevCards, cardCategories[cardIdx].cardMaterial);
            StartCoroutine(FlipPage(currentPage, currentCards, "Prev Page", rightAnchor));
            StartCoroutine(FlipPage(prevPage, nextCards, "Prev Page", centerAnchor));

            UpdateCatName();
        }
    } 
}
