using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeOut : MonoBehaviour
{
    [SerializeField]
    private Image imageToFade;
    [SerializeField]
    private TextMeshProUGUI textToFade;
    [SerializeField]
    private float fadeDuration = 2f;    

    private void Start()
    {
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        Color imageColor = imageToFade.color;
        Color textColor = textToFade.color;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            imageColor.a = alphaValue;
            textColor.a = alphaValue;

            imageToFade.color = imageColor;
            textToFade.color = textColor;

            yield return null;
        }

        imageColor.a = 0f;
        textColor.a = 0f;

        imageToFade.color = imageColor;
        textToFade.color = textColor;
    }
}