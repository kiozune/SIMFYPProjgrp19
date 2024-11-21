using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Question
{
    [SerializeField] public TMP_Text questionTxt;
    [SerializeField] public AnswerButton[] btns;
}

[System.Serializable]
public class AnswerButton
{
    [SerializeField] public TMP_Text txt;
    [SerializeField] public Toggle btn;
    [SerializeField] public bool isCorrect = false;
}

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
    [SerializeField] private GameObject[] prevCards;
    [SerializeField] private GameObject[] currentCards;
    [SerializeField] private GameObject[] nextCards;
    private int cardIdx = 0;

    [Header("Movement Anchors")]
    [SerializeField] private GameObject leftAnchor;
    [SerializeField] private GameObject centerAnchor;
    [SerializeField] private GameObject rightAnchor;

    [Header("Quiz Stuff")]
    [SerializeField] private GameObject quizBG;
    [SerializeField] private GameObject warningMsg;
    [SerializeField] private Question[] qns;
    [SerializeField] private GameObject resultsPage;
    [SerializeField] private TMP_Text score;

    [Header("Bool checks")]
    private bool isTurningPage = false;

    void Start()
    {
        // ensure cursor is enabled upon load
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

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

        // quiz stuff
        if (quizBG == null) Debug.LogError("Quiz background has not be set");
        else quizBG.SetActive(false);
        if (warningMsg == null) Debug.LogError("Warning message has not be set");
        else warningMsg.SetActive(false);
        if (resultsPage == null) Debug.LogError("Results page has not been set");
        else resultsPage.SetActive(false);
        if (score == null) Debug.LogError("Score text has not been set");
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
            Debug.Log(cardIdx);
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

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void StartQuiz()
    {
        ViewCard viewCardScript = currentPage.GetComponentsInChildren<ViewCard>()[0]; // just need one of them
        if (viewCardScript.GetCardsCollected() > 1)
        {
            quizBG.SetActive(true);
            SetQuestions();
        }
        else StartCoroutine(ShowWarning());
        
    }

    private void SetQuestions()
    {
        // ensure settings are reset
        for (int i = 0; i < qns.Length; i++)
        {
            qns[i].btns[0].isCorrect = false;
            qns[i].btns[1].isCorrect = false;
        }

        ViewCard viewCardScript = currentPage.GetComponentsInChildren<ViewCard>()[0]; // just need one of them
        List<string> questions = viewCardScript.GetQuizComponents(0);
        List<string> correct = viewCardScript.GetQuizComponents(1);
        List<string> wrong = viewCardScript.GetQuizComponents(2);
        List<int> cardIdxList = new();

        for (int i = 0; i < qns.Length; i++)
        {
            int attempts = 0;
            int maxAttempts = 50;  // Set a reasonable limit to prevent infinite loop
            bool addedIdx = false;
            while (!addedIdx)
            {
                int idx = Random.Range(0, questions.Count);
                if (!cardIdxList.Contains(idx))
                {
                    cardIdxList.Add(idx);
                    qns[i].questionTxt.SetText(questions[idx]);

                    // Randomize answer order
                    int btn = Random.Range(0, 2); // Should be 0 or 1
                    qns[i].btns[btn].txt.SetText(correct[idx]);
                    qns[i].btns[btn].isCorrect = true;
                    qns[i].btns[1 - btn].txt.SetText(wrong[idx]);

                    qns[i].btns[0].btn.isOn = false;
                    qns[i].btns[1].btn.isOn = false;
                    addedIdx = true;
                }

                // prevent infinite loop
                attempts++;
                if (attempts >= maxAttempts)
                {
                    Debug.LogError("Exceeded max attempts to find a unique question index.");
                    break;
                }
            }
        }
    }

    private IEnumerator ShowWarning()
    {
        warningMsg.SetActive(true);

        yield return new WaitForSeconds(3f);

        warningMsg.SetActive(false);
    }

    public void CancelQuiz()
    {
        quizBG.SetActive(false);
    }

    public void CloseResults()
    {
        resultsPage.SetActive(false);
    }

    public void SubmitQuiz()
    {
        resultsPage.SetActive(true);
        quizBG.SetActive(false);

        score.SetText(CalcScore().ToString());
    }

    private int CalcScore()
    {
        int marks = 0;
        for (int i = 0; i < qns.Length; i++)
        {
            for (int j = 0; j < qns[i].btns.Length; j++)
            {
                if (qns[i].btns[j].btn.gameObject.GetComponent<Toggle>().isOn && qns[i].btns[j].isCorrect)
                    marks++;
            }
        }
        return marks;
    }
}
