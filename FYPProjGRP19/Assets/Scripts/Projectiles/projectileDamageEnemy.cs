using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class projectileDamageEnemy : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Damage for the projectile to Player")]
    private float damage = 100f;

    // Setters for the damage and AOE toggle
    public void setDamage(float actualDamage)
    {
        damage = actualDamage;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
                // Single target damage
                ApplySingleTargetDamage(other.gameObject);
        }
    }

    private void ApplySingleTargetDamage(GameObject player)
    {
        PlayerHealth playerHP = player.GetComponent<PlayerHealth>();
        if (playerHP != null)
        {
            playerHP.takeDamage(damage);
        }
    }
}