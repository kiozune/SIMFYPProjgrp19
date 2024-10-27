using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardDropData
{
    [SerializeField,
        Tooltip("Level Name, primarily for tracking (dev)")] private string levelName;
    [SerializeField,
        Tooltip("Card background materials, in the order of level progression")] public Material cardMat;
    [SerializeField,
        Tooltip("Sprites for card drop images - for the UI")] public Sprite[] cardIcons;
}
public class CardDrop : MonoBehaviour
{
    [Header("Card Data")]
    [SerializeField,
        Tooltip("Level number\ni.e. 1300s = 0, 1400s-1600s = 1, Future level = 5"),
        Range(0, 4)] private int levelNum;
    [SerializeField] private CardDropData[] cardDropData;
    private int cardIdx;
    private string cardKey; // for PlayerPrefs

    private GameObject cardUI;
    private Image cardUIImg;
    private Vector3 displayAnchor;

    private bool isObtained = false;

    private void Start()
    {
        if (levelNum < 0 || levelNum > 5) Debug.LogError("Invalid level number entered");

        // set card material
        Renderer r = GetComponent<Renderer>();
        r.material = cardDropData[levelNum].cardMat;

        // set the specific card
        cardIdx = Random.Range(0, cardDropData[levelNum].cardIcons.Length);

        cardKey = "Card_" + levelNum.ToString() + "_" + cardIdx.ToString();
        Debug.Log(cardKey + " has spawned");

        cardUI = GameObject.Find("Card UI");
        if (cardUI == null) Debug.LogError("Card UI could not be found");

        cardUIImg = GameObject.Find("Card Image").GetComponent<Image>();
        if (cardUIImg == null) Debug.LogError("Card UI Image could not be found");

        displayAnchor = GameObject.Find("Display Anchor").transform.position;
        if (displayAnchor == null) Debug.LogError("Display Anchor could not be found"); 
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the model -  card is rotated, as the default model is laying on its back 
        transform.Rotate(Vector3.forward * Time.deltaTime * 50);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isObtained)
        {
            isObtained = true; // prevent multiple triggers

            if (PlayerPrefs.GetInt(cardKey, 0) == 0) // not yet obtained before
            {
                StartCoroutine(DisplayPopup());
                PlayerPrefs.SetInt(cardKey, 1);
            } 

            Destroy(gameObject); 
        }
    }

    private IEnumerator DisplayPopup()
    {
        // set the UI sprite to the corresponding image
        cardUIImg.sprite = cardDropData[levelNum].cardIcons[cardIdx];

        // display the UI
        float moveTime = 0.15f; // time to move the popup into view
        Vector3 startPos = cardUI.transform.position;
        float elapsedTime = 0;
        while (elapsedTime < moveTime)
        {
            cardUI.transform.position = Vector3.Lerp(startPos, displayAnchor, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2);

        // hide the UI
        elapsedTime = 0;
        while (elapsedTime < moveTime)
        {
            cardUI.transform.position = Vector3.Lerp(displayAnchor, startPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
