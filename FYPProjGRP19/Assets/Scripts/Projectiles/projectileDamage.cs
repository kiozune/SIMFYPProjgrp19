using EnemyInterface;
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
        // Try to get the IEnemy component from the collided object
        EnemyAI enemyAI = other.GetComponent<EnemyAI>();

        if (enemyAI != null)  // If the collided object is an enemy
        {
            if (isAOEEnabled)
            {
                // Apply full damage to the primary target and AOE damage to others
                ApplySingleTargetDamage(other.gameObject);
                ApplyAOEDamage(other.transform.position, other.gameObject);
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
        if (enemy != null)
        {
            enemyAI.TakeDamage(damage);
            if (enemyAI.checkHealth())
            {
                AwardPlayerEXP(enemyAI);
            }
        }
    }

    private void ApplyAOEDamage(Vector3 hitPosition, GameObject primaryTarget)
    {
        // Find all colliders in the AOE radius
        Collider[] hitEnemies = Physics.OverlapSphere(hitPosition, aoeRadius);

        foreach (Collider hit in hitEnemies)
        {
            if (hit.CompareTag("BasicEnemy") && hit.gameObject != primaryTarget)
            {
                EnemyAI enemyAI = hit.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    // Any other target than the primary will take 25% of the damage thrown
                    enemyAI.TakeDamage(damage * 0.25f);
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