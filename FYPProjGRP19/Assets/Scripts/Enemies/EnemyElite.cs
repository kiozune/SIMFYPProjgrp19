using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;
using EnemyInterface;

public class EnemyElite : MonoBehaviour, IEnemy
{
    [Header("Enemy Variables")]
    [SerializeField]
    public float maxHP = 200f;          //Enemy Max Health
    private float currentHP = 0f;       //Enemy Current Health
    public float damage = 30f;          //Enemy Damage
    public float attackRange = 5f;      //Enemy Attack Range
    public float attackCooldown = 2f;   //Enemy Cooldown
    private EnemyAI EnemyAI;            //get player reference from here
    private float attackTimer = 0f;     //attack cooldown controller
    public int expPts = 100;         //experience points given after defeat

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

        currentHP = maxHP;      //set currentHP to maxHP

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

    private void EnemyAttack()
    {
        animator.SetTrigger("Attack");

        PlayerHealth playerHP = playerTransform.gameObject.GetComponent<PlayerHealth>();
        if (playerHP != null)
        {
            playerHP.takeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
    }

    public int awardEXP()
    {
        return expPts;
    }
}
