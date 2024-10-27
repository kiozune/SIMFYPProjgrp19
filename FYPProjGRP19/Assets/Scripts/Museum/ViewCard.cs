using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[System.Serializable]
public class CardData
{ 
    [SerializeField] public string cardName;
    [SerializeField] public Sprite cardImg;
    [SerializeField] public string cardDesc;
}

[System.Serializable]
public class CardPages
{
    [SerializeField] private string catName;
    [SerializeField] public CardData[] cards;
}

public class ViewCard : MonoBehaviour
{
    [Header("Card Tracking")]
    [SerializeField] private CardPages[] cards;
    private int catIdx;
    [SerializeField] int cardIdx;
    private string cardKey;

    [Header("UI elements")]
    [SerializeField] private GameObject card;
    private Animator cardAnim;
    [SerializeField] private Image cardImg;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private GameObject viewUI;
    [SerializeField] private Transform zoomAnchor;
    [SerializeField] private TMP_Text cardNameTxt;
    [SerializeField] private TMP_Text cardDescTxt;
    [SerializeField] private Button closeBtn;
    private Vector3 startPos;
    private Vector3 startScale;

    // Start is called before the first frame update
    void Start()
    {
        if (cards.Length == 0) Debug.LogError(name + ": no card data has been assigned");
        catIdx = 0;
        CheckCardObtained();

        if (card == null) Debug.LogError("Card has not been assigned");
        if (cardImg == null) Debug.LogError("Card Image has not been assigned");
        if (defaultSprite == null) Debug.LogError("A default sprite has not been assigned");

        cardAnim = card.GetComponentInChildren<Animator>();
        if (cardAnim == null) Debug.LogError("Card animator could not be found");

        if (cardNameTxt == null) Debug.LogError("Card Title could not be found");
        if (cardDescTxt == null) Debug.LogError("Card Desc could not be found");
        if (closeBtn == null) Debug.LogError("Close Btn could not be found");
        if (viewUI == null) Debug.LogError("Zoom In Background could not be found");
        else viewUI.SetActive(false);
    }

    public void UpdateCard(int page)
    {
        cardAnim = card.GetComponentInChildren<Animator>(); // the animator gets reset for some reason
        catIdx = page;
        CheckCardObtained();
    }

    private void CheckCardObtained()
    {
        cardKey = "Card_" + catIdx.ToString() + "_" + cardIdx.ToString();
        Button btn = GetComponent<Button>(); // may not have a button, so account for a null value
        if (PlayerPrefs.GetInt(cardKey, 0) == 0) // not yet obtained
        {
            cardImg.sprite = defaultSprite;
            if (btn != null) btn.enabled = false;
        } else
        {
            cardImg.sprite = cards[catIdx].cards[cardIdx].cardImg;
            if (btn != null) btn.enabled = true;
        }
    }

    public void ZoomIn()
    {
        StartCoroutine(ZoomInCard());
    }

    private IEnumerator ZoomInCard()
    {
        // cardAnim.SetBool("isZoomedIn", true);
        float moveTime = 0.5f;
        startPos = card.transform.position;
        startScale = card.transform.localScale;
        float elapsedTime = 0;
        while (elapsedTime < moveTime)
        {
            card.transform.position = Vector3.Lerp(startPos, zoomAnchor.position, elapsedTime / moveTime);
            card.transform.localScale = Vector3.Lerp(startScale, startScale * 2, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (viewUI != null) viewUI.SetActive(true);
        else Debug.LogError("Zoom In Background could not be found");

        cardNameTxt.SetText(cards[catIdx].cards[cardIdx].cardName);
        cardDescTxt.SetText(cards[catIdx].cards[cardIdx].cardDesc);

        // set the onClick event on the close btn to this specific ZoomOut - account for specific transform.positions
        closeBtn.onClick.AddListener(() => ZoomOut()); 
    }

    public void ZoomOut()
    {
        StartCoroutine(ZoomOutCard());
    }

    private IEnumerator ZoomOutCard()
    {
        closeBtn.onClick.RemoveAllListeners(); // ensure the listener is removed
        viewUI.SetActive(false);

        // cardAnim.SetBool("isZoomedIn", false);
        float moveTime = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < moveTime)
        {
            card.transform.position = Vector3.Lerp(zoomAnchor.position, startPos, elapsedTime / moveTime);
            card.transform.localScale = Vector3.Lerp(startScale * 2, startScale, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
