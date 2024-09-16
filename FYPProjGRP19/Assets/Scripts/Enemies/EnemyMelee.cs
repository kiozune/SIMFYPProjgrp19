using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using EnemyInterface;

public class EnemyMelee : MonoBehaviour, IEnemy
{
    [Header("Enemy Variables")]
    [SerializeField]
    public float maxHP = 100f;          // Enemy Max Health
    private float currentHP = 0f;       // Enemy Current Health
    public float damage = 20f;          // Enemy Damage
    public float attackRange = 3f;      // Enemy Attack Range
    public float attackCooldown = 2f;   // Enemy Cooldown
    private float attackTimer = 0f;     // Attack cooldown controller
    public int expPts = 50;             // Experience points given after defeat

    //reference interface
    public int expGained { get; private set; }
    public float hp { get; private set; }

    // Inspector Elements
    private Transform playerTransform;
    private NavMeshAgent agent;
    private Animator animator;
    private Collider objCollider;

    // Start is called before the first frame update
    void Start()
    {
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

        //link to reference
        expGained = expPts;
        hp = currentHP;
    }

    // Update is called once per frame
    void Update()
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
        Debug.Log(currentHP);
    }

    public int awardEXP()
    {
        return expPts;
    }
}
