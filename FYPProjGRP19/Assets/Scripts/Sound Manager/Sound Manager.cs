using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;
    public AudioClip hitSoundClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevents this gameObject from being destroyed ever
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayHitSound()
    {
        if (audioSource != null && hitSoundClip != null)
        {
            audioSource.PlayOneShot(hitSoundClip);
        }
    }
}
