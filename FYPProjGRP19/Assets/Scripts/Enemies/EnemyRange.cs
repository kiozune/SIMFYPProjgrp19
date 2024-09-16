using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyInterface;

public class EnemyRange : MonoBehaviour, IEnemy
{
    [Header("Enemy Variables")]
    [SerializeField]
    public GameObject enemyProjectile;  //Enemy Projectile Prefab
    public Transform projectileSpawn;   //Where Projectile shoots from
    public float maxHP = 100f;          //Enemy Max Health
    private float currentHP = 0f;       //Enemy Current Health
    public float attackRange = 7f;      //Enemy Attack Range
    public float attackCooldown = 2f;   //Enemy Cooldown
    private EnemyAI EnemyAI;            //get player reference from here
    private float attackTimer = 0f;     //attack cooldown controller
    public int expPts = 50;             //experience points given after defeat

    //Projectile Variables
    private float projectileSpeed = 10f;

    //reference interface
    public int expGained { get; private set; }
    public float hp { get; private set; }

    //Inspector Elements
    private NavMeshAgent agent;
    private Animator animator;
    private Collider objCollider;
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("NavMeshAgent is not attached to the enemy.");

        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator component is not attached to the enemy.");

        objCollider = GetComponent<Collider>();
        if (objCollider == null) Debug.LogError("Parent object collider not found.");

        currentHP = maxHP;

        // Find the player by tag and set their transform
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found in the scene!");
        }

        //link to reference
        expGained = expPts;
        hp = currentHP;
    }


    // Update is called once per frame
    void Update()
    {
        if (currentHP > 0)      //if enemy has health
        {
            if (playerTransform != null && agent != null)    //if player and agent exists
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);     //checks distance btn player and enemy

                if (distanceToPlayer < attackRange)     //if player is in attack range
                {
                    agent.isStopped = true;     //stop enemy movement

                    if (attackTimer <= 0f)
                    {
                        EnemyAttack();

                        attackTimer = attackCooldown; //reset cooldown
                    }
                    else
                    {
                        attackTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(playerTransform.position);
                }
            }

            if (agent != null && animator != null)
            {
                if (agent.velocity.magnitude > 0.1f)
                {
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    animator.SetBool("isWalking", false);
                }
            }
        }
        else
        {
            agent.isStopped = true;
            animator.SetTrigger("Death");
            Destroy(gameObject, 2f);
        }
    }

    //enemy takes damage from projectile
    private void EnemyAttack()
    {
        animator.SetTrigger("Attack");

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

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log(currentHP);
    }

    public int awardEXP()
    {
        return expPts;
    }
}
