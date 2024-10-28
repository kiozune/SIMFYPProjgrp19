using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Canvas bossHealthCanvas;  // Reference to the health bar on canvas (if needed)
    public UnityEngine.UI.Slider healthSlider;  // Reference to health slider UI component

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Boss has died!");
        // You can add animations or other logic for boss death here
        Destroy(gameObject); // Or trigger death animation
    }
}
