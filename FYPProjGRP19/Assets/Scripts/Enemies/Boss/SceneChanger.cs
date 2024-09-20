using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public string Museumindoor;  // Name of the scene to load
    public Image fadeImage;        // Reference to the black UI Image for fading
    public float fadeDuration = 1f;  // Duration of the fade effect

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if the colliding object has the "Player" tag
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }
    IEnumerator FadeAndLoadScene()
    {
        // Start fading to black
        yield return StartCoroutine(FadeToBlack());

        // Wait for a short delay
        yield return new WaitForSeconds(0.5f);

        // Load the next scene
        SceneManager.LoadScene(Museumindoor);
    }

    IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}
