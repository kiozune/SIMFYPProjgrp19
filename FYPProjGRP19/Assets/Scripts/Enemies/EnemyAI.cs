using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;          // Reference to the player's transform
    public float maxHP = 100f;        // Enemy health points
    private float currentHP = 100f;
    public float damageFromProjectile = 20f;  // Amount of damage taken from each projectile hit
    public float attackDamage = 20f;  // Damage the enemy does to the player
    public float attackRange = 5f;    // Range within which the enemy can attack the player
    public float attackCooldown = 2f; // Time between each attack

    private NavMeshAgent agent;       // Reference to the NavMeshAgent component
    private Animator animator;        // Reference to the Animator component
    private Collider parentCollider;  // Reference to the Collider of the parent object
    private SliderBar sliderBar;

    private float attackTimer = 0f;   // Timer to control the attack cooldown

    [Header("Enemy EXP")]
    [SerializeField]
    private int experiencePoints = 50;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("NavMeshAgent is not attached to the enemy.");

        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator component is not attached to the enemy.");

        parentCollider = GetComponent<Collider>();
        if (parentCollider == null) Debug.LogError("Parent object collider not found.");

        if (player == null) Debug.LogError("Player is not assigned in the EnemyAI script.");

        sliderBar = GetComponentInChildren<SliderBar>();
        if (sliderBar == null) Debug.LogError("SliderBar is not assigned or found.");

        currentHP = maxHP;
    }

    void Update()
    {
        if (currentHP > 0)
        {
            if (player != null && agent != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                // If within attack range, stop moving and attack
                if (distanceToPlayer <= attackRange)
                {
                    agent.isStopped = true;

                    // Check if we can attack (based on cooldown)
                    if (attackTimer <= 0f)
                    {
                        AttackPlayer();
                        attackTimer = attackCooldown;  // Reset the attack cooldown
                    }
                    else
                    {
                        attackTimer -= Time.deltaTime;  // Decrease the timer
                    }
                }
                else
                {
                    // Move towards the player if out of range
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
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
            animator.SetTrigger("Death");
            Destroy(gameObject,2.2f);
        }
    }

    private void AttackPlayer()
    {
        animator.SetTrigger("Attack");  // Play attack animation

        PlayerHealth playerHP = player.gameObject.GetComponent<PlayerHealth>();
        if (playerHP != null)
        {
            playerHP.takeDamage(attackDamage);  // Apply damage to the player
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;  // Reduce the enemy's HP by the damage amount

        sliderBar.UpdateBar(currentHP, maxHP);

        Debug.Log("Enemy HP: " + currentHP);
    }

    public bool checkHealth()
    {
        return currentHP <= 0;
    }

    public float returnHealthValue()
    {
        return currentHP;
    }

    public int awardEXP()
    {
        return experiencePoints;
    }
}