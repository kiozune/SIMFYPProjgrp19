using UnityEngine;

public class WolfBossScript : MonoBehaviour
{
    public Transform player; 
    public float detectionRange = 10f; 
    public float moveSpeed = 2f; 
    public float stoppingDistance = 2f; 
    public float rotationSpeed = 5f; 
    public float attackCooldown = 2f; 
    public int damage = 10; 
    
    // Add HP-related variables
    public int maxHealth = 100;
    private int currentHealth;

    private bool isPlayerInRange = false;
    private bool isAttacking = false;
    private bool isCharging = false; // New flag for charge attack phase
    private bool isSecondPhase = false; // New flag for the 50% health threshold
    private float attackTimer = 0f;

    private Animator animator; 
    private PlayerHealth playerHealth; 

    public float chargeSpeed = 5f; // Speed during charge attack
    public float chargeCooldown = 5f; // Cooldown between charges in second phase
    private float chargeTimer = 0f; // Timer to manage charge cooldown

    void Start()
    {
        animator = GetComponent<Animator>();

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        // Initialize the boss's current health
        currentHealth = maxHealth;
    }

    void Update()
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    // Check if the boss should enter the second phase
    if (currentHealth <= maxHealth * 0.5f && !isSecondPhase)
    {
        EnterSecondPhase();
    }

    // Handle different phases of behavior
    if (isSecondPhase && chargeTimer <= 0f && !isCharging)
    {
        ChargeAttack();
    }
    else if (isPlayerInRange && distanceToPlayer > stoppingDistance && !isAttacking && !isCharging)
    {
        MoveTowardsPlayer();
    }
    else if (distanceToPlayer <= stoppingDistance && !isAttacking && attackTimer <= 0f && !isCharging)
    {
        AttackPlayer();
    }

    // Handle attack cooldown
    if (isAttacking)
    {
        attackTimer -= Time.deltaTime;

        // Ensure the attack animation finishes before setting the state back
        if (attackTimer <= 0f)
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
    }

    // Handle charge cooldown
    if (isCharging)
    {
        chargeTimer -= Time.deltaTime;

        if (chargeTimer <= 0f)
        {
            isCharging = false;
            animator.SetBool("isCharging", false); // Stop charging animation
            chargeTimer = chargeCooldown; // Reset cooldown for next charge
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
    // Prevent triggering attack multiple times if already attacking
    if (attackTimer <= 0f && !isCharging && !isAttacking)
    {
        isAttacking = true;
        animator.SetTrigger("Attack"); // Use SetTrigger instead of SetBool

        if (playerHealth != null)
        {
            playerHealth.takeDamage(damage);
        }

        attackTimer = attackCooldown; // Reset attack cooldown
    }
}
    void ChargeAttack()
    {
        isCharging = true;
        animator.SetBool("isCharging", true); // Start charging animation

        Vector3 chargeDirection = (player.position - transform.position).normalized;
        transform.position += chargeDirection * chargeSpeed * Time.deltaTime;

        chargeTimer = chargeCooldown;
    }

    // New function to handle taking damage
    public void takeDamage(int amount)
    {
        currentHealth -= amount;

        // Check if health drops to zero or below
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Function to handle the boss's death
    void Die()
    {
        // Play death animation
        animator.SetTrigger("Die");

        // Disable the boss (you can also destroy it)
        Destroy(gameObject, 2f); // Destroy after 2 seconds to allow death animation to play
    }

    void EnterSecondPhase()
    {
        isSecondPhase = true;
        // Any setup you want for the 50% health phase (like special effects or speed boosts)
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, detectionRange); 
    }
}
