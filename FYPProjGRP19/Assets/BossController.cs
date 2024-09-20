using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public Slider bossHealthBar;   // Reference to the Slider for the health bar
    public Text bossNameText;      // Reference to the Text for the boss name

    public string bossName = "Boss Name"; // Set this to the boss's name
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        // Set the boss's current health to max health at the start
        currentHealth = maxHealth;

        // Set the boss name in the UI
        bossNameText.text = bossName;

        // Set the max value for the health bar and initialize it to full health
        bossHealthBar.maxValue = maxHealth;
        bossHealthBar.value = currentHealth;

        // Hide the health bar initially (if the boss should only appear after 15 minutes)
        bossHealthBar.gameObject.SetActive(false);
        bossNameText.gameObject.SetActive(false);
    }

    // Call this function to show the boss health bar and name (after the 15 min mark)
    public void ActivateBossUI()
    {
        bossHealthBar.gameObject.SetActive(true);
        bossNameText.gameObject.SetActive(true);
    }

    // Call this function to deal damage to the boss
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        bossHealthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle the boss's death here (e.g., play death animation, trigger event, etc.)
        Debug.Log(bossName + " has been defeated!");
    }
}
