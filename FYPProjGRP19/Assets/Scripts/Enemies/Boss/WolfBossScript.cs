using UnityEngine;
using System.Collections;
public class WolfBossScript : MonoBehaviour
{
    public Transform player; 
    public float detectionRange = 10f; 
    public float attackRange = 2f; // New attack range variable
    public float moveSpeed = 2f; 
    public float rotationSpeed = 5f; 
    public float attackCooldown = 2f; 
    public int damage = 10; 
    
    public int maxHealth = 100;
    private int currentHealth;

    private bool isPlayerInRange = false;
    private bool isAttacking = false;
    private bool isCharging = false; 
    private bool isSecondPhase = false; 
    private float attackTimer = 0f;

    private Animator animator; 
    private PlayerHealth playerHealth; 

    public float chargeSpeed = 5f; 
    public float chargeCooldown = 5f; 
    private float chargeTimer = 0f; 

    void Start()
    {
        animator = GetComponent<Animator>();

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        currentHealth = maxHealth;
    }

    void Update()
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    isPlayerInRange = distanceToPlayer <= detectionRange;

    if (isPlayerInRange)
    {
        if (distanceToPlayer > attackRange && !isCharging)
        {
            // Player is out of attack range but within detection range
            MoveTowardsPlayer();
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false); // Stop attack animation if previously triggered
            isAttacking = false; // Ensure attacking is reset
        }
        else if (distanceToPlayer <= attackRange && attackTimer <= 0f && !isCharging)
        {
            // Player is in attack range
            AttackPlayer();
            animator.SetBool("isWalking", false);
        }
    }
    else
    {
        // Player is out of detection range, reset states
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false); // Ensure attack animation stops
        isAttacking = false; // Reset attack flag
    }

    // Handle attack cooldown timer
    if (isAttacking)
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            isAttacking = false;
        }
    }

    // Handle charge cooldown timer
    if (isCharging)
    {
        chargeTimer -= Time.deltaTime;
        if (chargeTimer <= 0f)
        {
            isCharging = false;
            chargeTimer = chargeCooldown;
        }
    }
}
     void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; 

        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        targetRotation *= Quaternion.Euler(0, 90, 0); 

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

  void AttackPlayer()
{
    if (attackTimer <= 0f && !isCharging && !isAttacking)
    {
        Debug.Log("AttackPlayer triggered!");
        isAttacking = true;
        animator.SetTrigger("Attack");
        animator.SetBool("isAttacking", true);
        animator.SetBool("isWalking", false);

        if (playerHealth != null)
        {
            playerHealth.takeDamage(damage);
            Debug.Log($"Player damaged by {damage}!");
        }

        attackTimer = attackCooldown; // Start cooldown
    }
}

    void ChargeAttack()
    {
        isCharging = true;
        animator.SetBool("isCharging", true);

        Vector3 chargeDirection = (player.position - transform.position).normalized;
        transform.position += chargeDirection * chargeSpeed * Time.deltaTime;

        chargeTimer = chargeCooldown;
    }

    public void takeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (!isSecondPhase && currentHealth <= maxHealth / 2)
        {
            EnterSecondPhase();
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
        Destroy(gameObject, 2f); 
    }

    void EnterSecondPhase()
    {
        isSecondPhase = true;
        // Add any second-phase logic here
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Detection range

        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, attackRange); // Attack range
    }
}
