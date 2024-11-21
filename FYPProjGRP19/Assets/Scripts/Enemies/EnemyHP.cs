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

    //[Header("Death Handling Script")]
    //[SerializeField]
    //private MonoBehaviour deathHandlerScript; // Reference to any script that implements HandleDeath

    private void Start()
    {
        currentHP = maxHP;
        nextHealthThreshold = maxHP * 0.6f;
        if (sliderBar == null)
        {
            sliderBar = GetComponentInChildren<SliderBar>();
        }
        if (sliderBar == null)
        {
            Debug.LogError("SliderBar is not assigned or found.");
        }

        // You can optionally log an error if no death handler is assigned, but this is not mandatory
       // if (deathHandlerScript == null) Debug.LogError("Death handler script is not assigned.");
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
            IsDead();
            EnemyAI enemyaiScript = GetComponent<EnemyAI>();
            if (enemyaiScript != null)
            {
                enemyaiScript.HandleDeath();
            }
            PirateEnemyAI enemeypirateAIScript = GetComponent<PirateEnemyAI>();
            if (enemeypirateAIScript != null)
            {
                enemeypirateAIScript.HandleDeath();
            }
        }
    }

    private void PlayHitSound()
    {
        if (!soundPlayed)
        {
            //SoundManager.Instance.PlayHitSound();
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

    //calculate percentage health to trigger next enemy boss phase
    public bool HalfHealth()
    {
        if (currentHP <= (maxHP / 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int AwardEXP()
    {
        return experiencePoints;
    }
}