using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [Header("Enemy Health Values")]
    [SerializeField]
    private float maxHP = 100f;         // Maximum Health Points
    [SerializeField]
    private float currentHP = 100f;     // Current Health Points
    [SerializeField]
    private float nextHealthThreshold;  // Threshold to play sound on low health

    [Header("VFX/SFX")]
    [SerializeField]
    private GameObject hitVFXPrefab;    // Visual effect for getting hit
    [SerializeField]
    private AudioClip hitSoundClip;     // Sound effect for getting hit
    [SerializeField]
    private AudioSource audioSource;    // Audio source for playing sound

    [Header("UI")]
    [SerializeField]
    private SliderBar sliderBar;        // Reference to the health bar

    private bool soundPlayed = false;   // Boolean to track if sound has been played on low health
    private bool isDead = false;        // Boolean to check if the enemy is dead

    [Header("EXP")]
    [SerializeField]
    private int experiencePoints = 50;  // EXP awarded when the enemy dies

    private EnemyAI enemyAI;            // Reference to EnemyAI script for interacting with other functionalities

    private void Start()
    {
        currentHP = maxHP;
        nextHealthThreshold = maxHP * 0.6f;

        sliderBar = GetComponentInChildren<SliderBar>();
        if (sliderBar == null) Debug.LogError("SliderBar is not assigned or found.");

        enemyAI = GetComponent<EnemyAI>();  // Get reference to EnemyAI
        if (enemyAI == null) Debug.LogError("EnemyAI is not assigned or found.");
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        sliderBar.UpdateBar(currentHP, maxHP);

        // Play hit VFX
        if (hitVFXPrefab != null)
        {
            Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);
        }

        if (currentHP <= nextHealthThreshold && !soundPlayed)
        {
            PlayHitSound();
        }

        if (currentHP <= 0)
        {
            // Trigger death event
            HandleDeath();
        }
    }
    private void PlayHitSound()
    {
        if (!soundPlayed)
        {
            
            SoundManager.Instance.PlayHitSound();
            soundPlayed = true;
        }
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }

    public float GetCurrentHealth()
    {
        return currentHP;
    }

    public int AwardEXP()
    {
        return experiencePoints;
    }

    private void HandleDeath()
    {
        if (!isDead)
        {
            // Call death handling from EnemyAI
            enemyAI.HandleDeath();
            isDead = true;
        }
    }
}