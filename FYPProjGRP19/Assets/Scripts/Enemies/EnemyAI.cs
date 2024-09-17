using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//Base Class
//All Enemy Types inherit from this script
//Enemy will track player using Tag in their own script (may be modified)

public class EnemyAI : MonoBehaviour
{
    protected float currentHP;
    protected float maxHP = 100f;
    protected int expPoints = 50;
    protected float attackRange = 2f;
    protected float attackCooldown = 2f;
    protected float attackTimer = 0f;
    //public Sprite normalEnemyImage;

    //Inspector Elements
    protected Transform playerTransform;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Collider objCollider;
    protected Sprite enemyImage;

    protected virtual void Awake()
    {
        //Get components from object
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("NavMeshAgent is not attached to the enemy.");

        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator component is not attached to the enemy.");

        objCollider = GetComponent<Collider>();
        if (objCollider == null) Debug.LogError("Parent object collider not found.");

        currentHP = maxHP;  // Set currentHP to maxHP

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
    }
    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    protected virtual void Update()
    {
        if (currentHP > 0)  // If enemy has health
        {
            if (playerTransform != null && agent != null)  // If player and agent exist
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);  // Check distance between player and enemy

                if (distanceToPlayer < attackRange)  // If player is in attack range
                {
                    agent.isStopped = true;  // Stop enemy movement

                    if (attackTimer <= 0f)
                    {
                        EnemyAttack();
                        attackTimer = attackCooldown;  // Reset cooldown
                    }
                    else
                    {
                        attackTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(playerTransform.position);  // Move towards the player
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
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            animator.SetTrigger("Death");
            Destroy(gameObject, 2f);
        }
    }

    protected virtual void EnemyAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log(currentHP);
    }

    public virtual int awardEXP()
    {
        return expPoints;
    }

    public float getHealth()
    {
        return currentHP;
    }
}