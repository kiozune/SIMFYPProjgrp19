using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRange : EnemyAI
{
    [Header("Enemy Variables")]
    [SerializeField]
    public GameObject enemyProjectile;  //Enemy Projectile Prefab
    public Transform projectileSpawn;   //Where Projectile shoots from
    private float projectileSpeed = 10f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        maxHP = 100f;
        currentHP = maxHP;
        expPoints = 100;
        attackRange = 5f;
    }

    protected override void EnemyAttack()
    {
        base.EnemyAttack();

        // Check if the projectile prefab and spawn point are valid
        if (enemyProjectile != null && projectileSpawn != null)
        {
            // Instantiate the projectile at the spawn point in front of the enemy
            GameObject projectile = Instantiate(enemyProjectile, projectileSpawn.position, Quaternion.identity);

            // Get the Rigidbody of the projectile and launch it toward the player
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate direction towards the player from the spawn point
                Vector3 direction = (playerTransform.position - projectileSpawn.position).normalized;
                rb.velocity = direction * projectileSpeed;  // Apply velocity to the projectile
                Debug.Log("Projectile launched from in front of the enemy!");
            }
        }
        else
        {
            Debug.LogError("Projectile prefab or spawn point is missing.");
        }
    }
}