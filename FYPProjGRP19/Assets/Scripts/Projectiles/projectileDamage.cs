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
        if (other.CompareTag("Enemy") || other.CompareTag("BasicEnemy"))
        {
            if (isAOEEnabled)
            {
                SoundManager.Instance.PlayExplosionSound();
                // Apply full damage to the primary target and AOE damage to others
                ApplySingleTargetDamage(other.gameObject);
                ApplyAOEDamage(other.transform.position, other.gameObject);
            }
            else
            {
                SoundManager.Instance.PlayArrowHitSound();
                // Single target damage
                ApplySingleTargetDamage(other.gameObject);
            }
        }
    }

    private void ApplySingleTargetDamage(GameObject enemy)
    {
        //Enemies
        EnemyHP enemyAI = enemy.GetComponent<EnemyHP>();
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(damage);
            if (enemyAI.GetCurrentHealth() < 0)
            {
                AwardPlayerEXP(enemyAI);
            }
            Destroy(gameObject);
        }
        //Pirates
        PirateEnemyAI pirateAI = enemy.GetComponent<PirateEnemyAI>();
        if (pirateAI != null)
        {
            pirateAI.TakeDamage(damage);
            if (pirateAI.checkHealth())
            {
                pirateAI.awardEXP();
            }
            Destroy(gameObject);
        }
        //Boss
        BossHealth bossHP = enemy.GetComponent<BossHealth>();
        if (bossHP != null)
        {
            bossHP.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    private void ApplyAOEDamage(Vector3 hitPosition, GameObject primaryTarget)
    {
        // Find all colliders in the AOE radius
        Collider[] hitEnemies = Physics.OverlapSphere(hitPosition, aoeRadius);

        foreach (Collider hit in hitEnemies)
        {
            if (hit.CompareTag("Enemy") && hit.gameObject != primaryTarget)
            {
                EnemyHP enemyAI = hit.GetComponent<EnemyHP>();
                if (enemyAI != null)
                {
                    // Any other target than the primary will take 25% of the damage thrown
                    enemyAI.TakeDamage(damage * 0.25f);
                    if (enemyAI.GetCurrentHealth() < 0)
                    {
                        AwardPlayerEXP(enemyAI);
                    }
                    Destroy(gameObject);
                }
            }
        }
    }

    private void AwardPlayerEXP(EnemyHP enemyAI)
    {
        // Find the player and award experience points
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerLevel playerLevel = player.GetComponent<PlayerLevel>();
            if (playerLevel != null)
            {
                playerLevel.AddEXP(enemyAI.AwardEXP());
                playerLevel.UpdateXPSlider();
            }
        }
    }
}