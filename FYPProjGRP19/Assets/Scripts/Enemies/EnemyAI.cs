using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField]
    public Transform player;           // Reference to the player's transform
    [SerializeField]
    private GameObject playerPrefab;

    [Header("Rock prefab values")]
    [SerializeField]
    private GameObject rockPrefab;      // The rock projectile prefab
    [SerializeField]
    private Transform throwPoint;       // The point from which the rock is thrown

    [Header("Enemy Attributes")]
    [SerializeField]
    private float attackDamage = 20f;   // Damage the enemy does to the player
    [SerializeField]
    private float attackRange = 5f;     // Attack range
    [SerializeField]
    private float attackCooldown = 2f;  // Time between each attack
    [SerializeField]
    private float jumpCooldown = 20f;   // Time between jumps
    [SerializeField]
    private float jumpDistance = 6f;    // Jump distance

    [Header("Enemy Type")]
    [SerializeField]
    private bool canJump = true;
    [SerializeField]
    private bool rangeEnemy;
    [SerializeField]
    private bool normalEnemy;
    [SerializeField]
    private bool eliteEnemy;
    private bool isThrowing = false;
    
    [Header("UI")]
    [SerializeField]
    private Sprite normalEnemyImage;
    [SerializeField]
    private Sprite rangeImage;
    [SerializeField]
    private Image mobIcon;

    private NavMeshAgent agent;        // NavMeshAgent for movement
    private Animator animator;         // Animator for animations
    private float attackTimer = 0f;    // Timer for attack cooldown
    private float jumpTimer = 0f;      // Timer for jump cooldown

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        player = playerPrefab.transform;
        
        if (rangeEnemy) changeRangeIcon();
        if (normalEnemy) changeMeleeIcon();
    }

    private void Update()
    {
        if (!GetComponent<EnemyHP>().IsDead())
        {
            HandleMovement();
            HandleAttacksAndThrows();
        }

        HandleMovementAnimation();

        // Jump cooldown
        if (!canJump)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f)
            {
                canJump = true;
            }
        }
    }

    private void HandleMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (eliteEnemy && distanceToPlayer <= jumpDistance && canJump)
        {
            JumpToPlayer();
        }
        else if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    private void HandleAttacksAndThrows()
    {
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

    private void AttackPlayer()
    {
        animator.SetTrigger("Attack");
        PlayerHealth playerHP = player.GetComponent<PlayerHealth>();
        if (playerHP != null)
        {
            playerHP.takeDamage(attackDamage);
        }
    }

    private void ThrowProjectile()
    {
        if (isThrowing) return;

        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        animator.SetTrigger("Attack");

        StartCoroutine(ThrowProjectileWithDelay(1.25f));
        isThrowing = true;
    }

    private IEnumerator ThrowProjectileWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject rock = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);
        ProjectileDirEnemy projectile = rock.GetComponent<ProjectileDirEnemy>();

        if (projectile != null)
        {
            Vector3 throwDirection = CalculateThrowDirection(throwPoint.position, player.position);
            projectile.SetDirection(throwDirection);
        }

        isThrowing = false;
    }

    private Vector3 CalculateThrowDirection(Vector3 start, Vector3 target)
    {
        Vector3 direction = (target - start).normalized;
        direction.y = 0.5f; // Arc for projectile
        return direction;
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
    public void HandleDeath()
    {
        // Stop enemy movement and handle death logic
        agent.isStopped = true;
        GetComponent<CapsuleCollider>().enabled = false;

        animator.SetTrigger("Death");
        StartCoroutine(HandleDeathAfterAnimation());
    }

    private IEnumerator HandleDeathAfterAnimation()
    {
        yield return new WaitForSeconds(2.2f);

        LootDrops lootDrop = GetComponent<LootDrops>();
        if (lootDrop != null)
        {
            lootDrop.DropLoot();
        }

        Destroy(gameObject);
    }

    private void HandleMovementAnimation()
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

    private void changeMeleeIcon()
    {
        mobIcon.sprite = normalEnemyImage;
    }

    private void changeRangeIcon()
    {
        mobIcon.sprite = rangeImage;
    }
}