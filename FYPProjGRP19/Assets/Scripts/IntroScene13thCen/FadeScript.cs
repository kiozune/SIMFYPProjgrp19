using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    
    // Assign the TextMeshProUGUI component in the inspector
    public TextMeshProUGUI textToFade; 
    
    // Assign the Image component for the black background
    public Image background;      
    
    // Duration of the fade-in effect
    public float fadeInDuration = 3.0f;
    
    // Duration to display the text and background before fading out
    public float displayDuration = 3.0f;
    
    // Duration of the fade-out effect
    public float fadeOutDuration = 3.0f; // Duration of the fade-out effect

    // Start the fade-in process
    private void Start()
    {
        
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        // Fade In
        yield return Fade(0f, 1f, fadeInDuration);

        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        yield return Fade(1f, 0f, fadeOutDuration);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            // Gradually change the alpha value
            float alpha = Mathf.Lerp(startAlpha, endAlpha, currentTime / duration); 
            SetTextAlpha(alpha);
            SetBackgroundAlpha(alpha);
            yield return null;  
        }

        // Ensure final alpha is set
        SetTextAlpha(endAlpha);
        SetBackgroundAlpha(endAlpha);
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = textToFade.color;
        color.a = alpha;
        // Set the alpha value of the text
        textToFade.color = color;  
    }

    private void SetBackgroundAlpha(float alpha)
    {
        Color color = background.color;
        color.a = alpha;
        // Set the alpha value of the background
        background.color = color; 
    }
}