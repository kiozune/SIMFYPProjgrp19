using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicManager : MonoBehaviour
{
    private float animLength = 14.4f;
    private bool isSkipped = false;
    private bool isWatching = false;
    private bool isSkipping = false;
    private float skipTimer = 1.5f;
    [SerializeField] private Slider slider;

    private void Start()
    {
        if (slider != null) slider.gameObject.SetActive(false);
        else Debug.LogError("Slider could not be found");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSkipped)
        {
            if (!isWatching) StartCoroutine(WatchCutscene());
            if (!isSkipping && Input.GetMouseButtonDown(0)) StartCoroutine(SkipCutscene());
        }
    }

    private IEnumerator SkipCutscene()
    {
        isSkipped = true;
        bool skipFlag = true;
        slider.gameObject.SetActive(true);
        for (float i = 0; i < skipTimer; i += Time.deltaTime)
        {
            if (!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
            {
                skipFlag = false;
                break;
            } else
            {
                slider.value += Time.deltaTime / skipTimer;
                yield return null;
            }
        }
        if (skipFlag)
            SceneManager.LoadScene("Main Menu");
        else
        {
            isSkipping = false;
            slider.value = 0;
            slider.gameObject.SetActive(false);
        }
    }

    private IEnumerator WatchCutscene()
    {
        isWatching = true;
        yield return new WaitForSeconds(animLength);

        isSkipped = true;
        SceneManager.LoadScene("Main Menu");
    }
}
