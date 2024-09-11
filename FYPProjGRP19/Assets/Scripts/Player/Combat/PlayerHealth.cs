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
    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthSlider();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
