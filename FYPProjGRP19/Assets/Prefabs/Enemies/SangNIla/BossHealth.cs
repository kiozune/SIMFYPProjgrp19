using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;  // Set to public for access in WeaponDamageScript

    public Canvas bossHealthCanvas;  // Reference to the health bar on canvas
    public Slider healthSlider;  // Reference to health slider UI component

    [SerializeField]
    private string nextScene;

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

    public float GetCurrentHealth()
    {
        return currentHealth;
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
        SceneManager.LoadScene(nextScene);
        Destroy(gameObject); // Trigger death
    }
}
