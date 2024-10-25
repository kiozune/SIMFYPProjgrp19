using UnityEngine;
using UnityEngine.AI;

public class BossAttack : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float rotationSpeed = 5f; // Speed at which the boss turns to face the player

    private float attackTimer;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Check if player reference is assigned
        if (player == null)
        {
            Debug.LogError("Player reference not set in BossAttack script!");
        }

        attackTimer = 0f;
    }

 void Update()
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    Debug.Log("Distance to player: " + distanceToPlayer);

    // Rotate towards the player
    Vector3 directionToPlayer = (player.position - transform.position).normalized;
    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

    // Handle movement and attack logic
    if (distanceToPlayer <= detectionRange)
    {
        if (distanceToPlayer > attackRange)
        {
            // Move towards the player
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(player.position);

            // Update walking animation
            if (navMeshAgent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            // Attack the player
            navMeshAgent.isStopped = true; // Stop moving
            HandleAttack();
        }
    }
    else
    {
        // Player out of range, go to idle state
        navMeshAgent.isStopped = true; // Stop moving
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false); // Ensure attacking is false
    }

    // Decrement attack timer if it's greater than zero
    if (attackTimer > 0f)
    {
        attackTimer -= Time.deltaTime;
    }
}

void HandleAttack()
{
    if (attackTimer <= 0f) // Check if the cooldown has expired
    {
        Attack();
        attackTimer = attackCooldown; // Reset timer after attacking
    }

    // Interrupt walking animation for attacking
    animator.SetBool("isWalking", false);
    animator.SetBool("isAttacking", true);
}

void Attack()
{
    Debug.Log("Boss attacks!");
    // Add attack logic (damage, etc.)
}


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
