using UnityEngine;
using UnityEngine.AI;

public class BossAttack : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float rotationSpeed = 5f;
    public float attackDamage = 50f; // Damage dealt to the player

    private float attackTimer;
    private bool isAttacking = false;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            Debug.LogError("Player reference not set in BossAttack script!");
        }

        attackTimer = 0f;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Rotate towards the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // Handle movement and attack logic
        if (distanceToPlayer <= detectionRange && !isAttacking)
        {
            if (distanceToPlayer > attackRange)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(player.position);

                if (navMeshAgent.velocity.magnitude > 0.1f)
                {
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isAttacking", false);
                    animator.SetBool("isIdle", false);
                }
            }
            else if (attackTimer <= 0f)
            {
                navMeshAgent.isStopped = true;
                HandleAttack();
            }
        }
        else if (!isAttacking)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isIdle", false);
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void HandleAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            animator.SetBool("isIdle", false);
            Attack();
        }
    }

    void Attack()
{
    Debug.Log("Boss attacks!");

    // Deal damage to player
    PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
    if (playerHealth != null)
    {
        playerHealth.takeDamage(attackDamage);
    }

    // Directly invoke EndAttack, or ensure the animator transitions back to idle
    Invoke("EndAttack", animator.GetCurrentAnimatorStateInfo(0).length);
}

    void EndAttack()
{
    isAttacking = false;
    attackTimer = attackCooldown;
    animator.SetBool("isAttacking", false);
    animator.SetBool("isWalking", false); // This will return to idle if not moving
    animator.SetBool("isIdle", false);
}
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
