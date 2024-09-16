using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class projectileDamage : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Damage for the projectile to enemy")]
    private float damage = 55f;

    [SerializeField]
    [Tooltip("Enable or disable AOE damage")]
    private bool isAOEEnabled = false;  // Toggle for AOE damage

    [SerializeField]
    [Tooltip("Radius for AOE effect if enabled")]
    private float aoeRadius = 5f;       // Radius for AOE effect

    // Setters for the damage and AOE toggle
    public void setDamage(float actualDamage)
    {
        damage = actualDamage;
    }

    public void setAOEEnabled(bool enableAOE, float addRadiusExplosion)
    {
        if (!isAOEEnabled)
        {
            isAOEEnabled = enableAOE;
        }
        else
        {
            aoeRadius += addRadiusExplosion;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BasicEnemy"))
        {
            if (isAOEEnabled)
            {
                // Apply AOE damage to all enemies within a radius
                ApplyAOEDamage(other.transform.position);
            }
            else
            {
                // Single target damage
                ApplySingleTargetDamage(other.gameObject);
            }
        }
    }

    private void ApplySingleTargetDamage(GameObject enemy)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(damage);
            if (enemyAI.checkHealth())
            {
                AwardPlayerEXP(enemyAI);
            }
        }
    }

    private void ApplyAOEDamage(Vector3 hitPosition)
    {
        // Find all colliders in the AOE radius
        Collider[] hitEnemies = Physics.OverlapSphere(hitPosition, aoeRadius);

        foreach (Collider hit in hitEnemies)
        {
            if (hit.CompareTag("BasicEnemy"))
            {
                EnemyAI enemyAI = hit.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    // Apply damage to each enemy in range
                    enemyAI.TakeDamage(damage);
                    if (enemyAI.checkHealth())
                    {
                        AwardPlayerEXP(enemyAI);
                    }
                }
            }
        }
    }

    private void AwardPlayerEXP(EnemyAI enemyAI)
    {
        // Find the player and award experience points
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerLevel playerLevel = player.GetComponent<PlayerLevel>();
            if (playerLevel != null)
            {
                playerLevel.AddEXP(enemyAI.awardEXP());
                playerLevel.UpdateXPSlider();
            }
        }
    }
}