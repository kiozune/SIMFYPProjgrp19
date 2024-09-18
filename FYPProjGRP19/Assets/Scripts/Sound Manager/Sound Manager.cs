using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;
    [SerializeField]
    private AudioClip hitSoundClip;
    [SerializeField]
    private AudioClip shootingSoundClip;
    [SerializeField]
    private AudioClip explosionClip;
    [SerializeField]
    private AudioClip arrowHitClip;
    [SerializeField]
    private AudioClip fishmanDeathClip;

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
    public void PlayShootSound()
    {
        if (audioSource != null && shootingSoundClip != null)
        {
            audioSource.PlayOneShot(shootingSoundClip);
        }
    }
    public void PlayExplosionSound()
    {
        if (audioSource != null && explosionClip != null)
        {
            audioSource.PlayOneShot(explosionClip);
        }
    }
    public void PlayArrowHitSound()
    {
        if (audioSource != null && arrowHitClip != null)
        {
            audioSource.PlayOneShot(arrowHitClip);
        }
    }
    public void PlayDeathSound()
    {
        if (audioSource != null && fishmanDeathClip != null)
        {
            audioSource.PlayOneShot(fishmanDeathClip);
        }
    }
}
