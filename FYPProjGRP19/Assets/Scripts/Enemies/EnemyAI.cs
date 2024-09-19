using UnityEngine;
using UnityEngine.AI;

public enum AttackType
{
    Melee,
    Range,
    Elite
}

public class EnemyAI : MonoBehaviour
{
    public Transform player;                  // Reference to the player's transform
    public float maxHP = 100f;                // Enemy health points
    private float currentHP = 100f;
    public float damageFromProjectile = 20f;  // Amount of damage taken from each projectile hit
    public float meleeAttackDamage = 20f;     // Melee attack damage
    public float rangedAttackRange = 10f;     // Range at which the ranged enemy shoots projectiles
    public float meleeAttackRange = 3f;       // Range for melee attacks
    public float attackCooldown = 2f;         // Time between each attack

    private NavMeshAgent agent;               // Reference to the NavMeshAgent component
    private Animator animator;                // Reference to the Animator component
    private Collider parentCollider;          // Reference to the Collider of the parent object
    private SliderBar sliderBar;

    private float attackTimer = 0f;           // Timer to control the attack cooldown

    [Header("Enemy EXP")]
    [SerializeField]
    private int experiencePoints = 50;

    [Header("Attack Type")]
    public AttackType attackType;             // The type of attack the enemy performs
    public GameObject projectilePrefab;       // Projectile prefab for ranged attack
    public Transform projectileSpawnPoint;    // The spawn point for projectiles
    public float projectileSpeed = 10f;       // Speed of the projectile
    public float eliteHPBonus = 50f;          // Extra HP for Elite enemies
    public float eliteAttackBonus = 10f;      // Extra attack damage for Elite enemies

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

        // Adjust stats based on attack type
        if (attackType == AttackType.Elite)
        {
            currentHP += eliteHPBonus;        // Extra health for elite enemies
            meleeAttackDamage += eliteAttackBonus; // Extra melee damage for elite enemies
        }
    }

    void Update()
    {
        if (currentHP > 0)
        {
            if (player != null && agent != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                // Attack based on attack type and distance
                switch (attackType)
                {
                    case AttackType.Melee:
                        HandleMeleeBehavior(distanceToPlayer);
                        break;
                    case AttackType.Range:
                        HandleRangedBehavior(distanceToPlayer);
                        break;
                    case AttackType.Elite:
                        HandleEliteBehavior(distanceToPlayer);
                        break;
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
            Destroy(gameObject, 2.2f);
        }
    }

    // Handle melee enemies' behavior
    private void HandleMeleeBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer <= meleeAttackRange)
        {
            agent.isStopped = true;

            if (attackTimer <= 0f)
            {
                PerformMeleeAttack();
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
            agent.SetDestination(player.position);
        }
    }

    // Handle ranged enemies' behavior
    private void HandleRangedBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer <= rangedAttackRange)
        {
            agent.isStopped = true;

            if (attackTimer <= 0f)
            {
                PerformRangedAttack();
                attackTimer = attackCooldown;  // Reset cooldown
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }
        }
        else
        {
            // Keep ranged enemy at a comfortable distance
            if (distanceToPlayer > rangedAttackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
    }

    // Handle elite enemies' behavior (stronger melee)
    private void HandleEliteBehavior(float distanceToPlayer)
    {
        HandleMeleeBehavior(distanceToPlayer);
    }

    private void PerformMeleeAttack()
    {
        animator.SetTrigger("Attack");  // Play melee attack animation

        PlayerHealth playerHP = player.gameObject.GetComponent<PlayerHealth>();
        if (playerHP != null)
        {
            playerHP.takeDamage(meleeAttackDamage);  // Apply melee damage to the player
        }
    }

    private void PerformRangedAttack()
    {
        animator.SetTrigger("Attack");  // Play ranged attack animation

        // Check if the projectile prefab and spawn point are valid
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            // Instantiate the projectile at the spawn point in front of the enemy
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            // Get the Rigidbody of the projectile and launch it toward the player
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate direction towards the player from the spawn point
                Vector3 direction = (player.position - projectileSpawnPoint.position).normalized;
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
        currentHP -= damage;  // Reduce the enemy's HP by the damage amount
        //sliderBar.UpdateBar(currentHP, maxHP);

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
