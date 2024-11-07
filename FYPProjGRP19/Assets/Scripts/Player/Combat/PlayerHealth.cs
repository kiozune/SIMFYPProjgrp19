using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Values")]
    [SerializeField]
    private float currHealth = 1000;
    [SerializeField]
    private float maxHealth = 1000;

    [Header("UI Elements")]
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthSlider();
        gameOverScreen = GameObject.FindGameObjectWithTag("Gameover");
    }

    // Update is called once per frame
    void Update()
    {
        if (currHealth <= 0)
        {
            gameOverScreen.GetComponent<GameOver>().GameOverScreen();
        }
    }
    public void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currHealth / maxHealth;  // Set slider value based on XP ratio
        }
    }
    public void takeDamage(float damage)
    {
        currHealth -= damage;
        UpdateHealthSlider();
    }
    public void addHealth(float hp)
    {
        if (currHealth != maxHealth)
        {
            if (maxHealth - currHealth > hp)
            {
                currHealth += hp;
                UpdateHealthSlider();
            }
            else
            {
                currHealth += (maxHealth - currHealth);
                UpdateHealthSlider();
            }
        }
    }
}
