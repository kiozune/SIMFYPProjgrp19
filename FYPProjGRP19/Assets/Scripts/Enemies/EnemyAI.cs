using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Player Transforma value")]
    [SerializeField]
    public Transform player;          // Reference to the player's transform
    [SerializeField]
    private GameObject playerPrefab;
    [Header("Rock prefab values")]
    [SerializeField]
    private GameObject rockPrefab;     // The rock projectile prefab
    [SerializeField]
    private Transform throwPoint;      // The point from which the rock is thrown

    [Header("Enemy attribute Values")]
    [SerializeField]
    private float maxHP = 100f;        // Enemy health points
    [SerializeField]
    private float currentHP = 100f;
    [SerializeField]
    private float damageFromProjectile = 20f;  // Amount of damage taken from each projectile hit
    [SerializeField]
    private float attackDamage = 20f;  // Damage the enemy does to the player
    [SerializeField]
    private float attackRange = 5f;    // Range within which the enemy can attack the player
    [SerializeField]
    private float attackCooldown = 2f; // Time between each attack
    [SerializeField]
    private float nextHealthThreshold;
    [SerializeField]
    private float jumpCooldown = 20f;
    [SerializeField]
    private float jumpTimer = 0f;

    [SerializeField]
    private EnemyHP enemyHPscript;

    [SerializeField]
    private float jumpDistance = 6f;

    [Header("Enemy Bool Checks")]
    [SerializeField]
    private bool canJump = true; 
    [SerializeField]
    private bool isThrowing = false;
    [SerializeField]
    private bool rangeEnemy;
    [SerializeField]
    private bool normalEnemy;
    [SerializeField]
    private bool eliteEnemy;
    [SerializeField]
    private bool soundPlayed = false;
    [SerializeField]
    private bool hasDroppedLoot = false;
    [SerializeField]
    private bool isDead = false;
    [Header("UI")]
    [SerializeField]
    private Sprite normalEnemyImage;
    [SerializeField]
    private Sprite rangeImage;
    [SerializeField]
    private Image mobIcon;
    
    [Header("VFX/SFX")]
    [SerializeField] 
    private GameObject hitVFXPrefab;
    [SerializeField] 
    private AudioClip hitSoundClip;
    [SerializeField]
    private AudioSource audioSource;

    [Header("NavMesh Agent")]
    [SerializeField]
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

        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        player = playerPrefab.transform;
        if (player == null) Debug.LogError("Player is not assigned in the EnemyAI script.");

        sliderBar = GetComponentInChildren<SliderBar>();
        if (sliderBar == null) Debug.LogError("SliderBar is not assigned or found.");

        currentHP = maxHP;
        if (rangeEnemy)
        {
            changeRangeIcon();
        }
        if(normalEnemy)
        {
            changeMeleeIcon();
        }
        //Setting the threshold for SFX
        nextHealthThreshold = maxHP * 0.6f;
        enemyHPscript = this.gameObject.GetComponent<EnemyHP>();
    }

    void Update()
    {
        if (currentHP > 0)
        {
            if (player != null && agent != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (eliteEnemy && distanceToPlayer <= jumpDistance && canJump)
                {
                    JumpToPlayer();
                }
                else if (distanceToPlayer <= attackRange)
                {
                    agent.isStopped = true;

                    if (attackTimer <= 0f)
                    {
                        if (rangeEnemy)
                        {
                            ThrowProjectile();
                        }
                        else
                        {
                            AttackPlayer();
                        }
                        attackTimer = attackCooldown;
                    }
                    else
                    {
                        attackTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    // If the player is out of range, AI will move
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

            // Handle jump cooldown
            if (!canJump)
            {
                jumpTimer -= Time.deltaTime;
                if (jumpTimer <= 0f)
                {
                    canJump = true; // Reset jump ability
                }
            }
        }
    }
    private void MoveTowardsPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }
    private void HandleAttackOrThrow()
    {
        if (attackTimer <= 0f)
        {
            if (rangeEnemy)
            {
                ThrowProjectile();  // For ranged enemies
            }
            else
            {
                AttackPlayer();  // For melee enemies
            }
            attackTimer = attackCooldown;  // Reset attack cooldown
        }
        else
        {
            attackTimer -= Time.deltaTime;  // Decrease attack timer
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

        //Play the VFX on the enemy Body
        if (hitVFXPrefab != null)
        {
            Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);
        }

      
        if (currentHP <= nextHealthThreshold)
        {
            // Play sound  below threshold
            PlayHitSound();
        }
        if (currentHP <= 0 || enemyHPscript.IsDead())
        {
            HandleDeath();  // Trigger death when health is 0 or below
        }
        else if (currentHP <= nextHealthThreshold && !soundPlayed)
        {
            PlayHitSound();
        }
    }
    private void PlayHitSound()
    {
        if (!soundPlayed)
        {
            SoundManager.Instance.PlayHitSound();
            soundPlayed = true;
        }
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
    private IEnumerator ThrowProjectileWithDelay(float delay)
    {
        // Wait
        yield return new WaitForSeconds(delay);

        GameObject rock = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);

        ProjectileDirEnemy projectile = rock.GetComponent<ProjectileDirEnemy>();

        if (projectile != null)
        {
            Vector3 targetPosition = player.position;
            Vector3 throwDirection = CalculateThrowDirection(throwPoint.position, targetPosition);

            // Set the direction of the projectile
            projectile.SetDirection(throwDirection);
        }
        isThrowing = false;
    }
    private void ThrowProjectile()
    {
        if (isThrowing)
        {
            return;
        }
        // Rotate the enemy to face the player
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0; // <- safety it doesnt rotate Y Axis
        transform.rotation = Quaternion.LookRotation(lookDirection);

        animator.SetTrigger("Attack");

        // Start the coroutine to instantiate the rock after a delay
        StartCoroutine(ThrowProjectileWithDelay(1.25f)); // 2-second delay
        isThrowing = true;
    }

    // Simplified throw direction calculation with an arc
    private Vector3 CalculateThrowDirection(Vector3 start, Vector3 target)
    {
        // Get the direction to the player
        Vector3 direction = (target - start).normalized;

        // Give it an upward arc
        direction.y = 0.5f;

        return direction;
    }
    private void changeMeleeIcon()
    {
        mobIcon.sprite = normalEnemyImage;

    }
    private void changeRangeIcon()
    {
        mobIcon.sprite = rangeImage;
    }
    private void JumpToPlayer()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            // Rotate to face the player
            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDirection);

            // Trigger jump animation
            animator.SetTrigger("Jump");

            // Temporarily disable NavMeshAgent to allow custom jump movement
            agent.isStopped = true;
            agent.enabled = false;

            // Start the jump movement
            StartCoroutine(HandleJump());
            canJump = false; // Disable jumping
            jumpTimer = jumpCooldown; // Start jump cooldown
        }
    }
    private IEnumerator HandleJump()
    {
        float jumpDuration = 1f;  // Duration of the jump
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;

        // Define the target position slightly above the player's position
        Vector3 targetPosition = player.position;
        targetPosition.y = startPosition.y; // Keep the same height

        // Calculate the distance to jump
        float jumpHeight = 2f;  // Adjust this value for height
        Vector3 jumpArcOffset = new Vector3(0, jumpHeight, 0); // Jump height offset

        // Disable the NavMeshAgent for jump
        agent.enabled = false;

        while (elapsedTime < jumpDuration)
        {
            // Move the enemy in the jump arc
            float t = elapsedTime / jumpDuration;
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t) + jumpArcOffset * Mathf.Sin(t * Mathf.PI); // Arc effect

            transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the final position after the jump
        transform.position = targetPosition;

        // Re-enable the NavMeshAgent after landing
        agent.enabled = true;
        agent.isStopped = false;  // Allow movement again
    }
    private IEnumerator HandleDeathAfterAnimation()
    {
        //Wait for 2.2 seconds before running the rest of the function
        yield return new WaitForSeconds(2.2f);

        if (!hasDroppedLoot)
        {
            LootDrops lootDrop = GetComponent<LootDrops>();
            if (lootDrop != null)
            {
                lootDrop.DropLoot();
            }

            hasDroppedLoot = true;
        }

        // Destroy the enemy game object after loot drop
        Destroy(gameObject);
    }
    public void HandleDeath()
    {
        // Stop the enemy from moving and disable the NavMeshAgent
        agent.isStopped = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        // Trigger death animation
        if (!isDead)
        {
            SoundManager.Instance.PlayDeathSound();  // Play death sound
            isDead = true;
        }

        animator.SetTrigger("Death");  // Play death animation
        StartCoroutine(HandleDeathAfterAnimation());  // Coroutine to handle death cleanup
    }
    private void HandleMovementAnimation()
    {
        if (agent != null && animator != null)
        {
            // Check if the agent is moving by checking its velocity
            if (agent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("isWalking", true);  // Start walk animation
            }
            else
            {
                animator.SetBool("isWalking", false);  // Stop walk animation
            }
        }
    }
}