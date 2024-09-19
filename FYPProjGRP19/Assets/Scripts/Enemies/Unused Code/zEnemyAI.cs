/*using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//Base Class
//All Enemy Types inherit from this script
//Enemy will track player using Tag in their own script (may be modified)
//Add Methods here

public class zEnemyAI : MonoBehaviour
{
    public Transform player;          // Reference to the player's transform
    public GameObject rockPrefab;     // The rock projectile prefab
    public Transform throwPoint;      // The point from which the rock is thrown
    public float maxHP = 100f;        // Enemy health points
    private float currentHP = 100f;
    public float damageFromProjectile = 20f;  // Amount of damage taken from each projectile hit
    public float attackDamage = 20f;  // Damage the enemy does to the player
    public float attackRange = 5f;    // Range within which the enemy can attack the player
    public float attackCooldown = 2f; // Time between each attack
    [Header("Enemy Bool Checks")]
    [SerializeField]
    private bool isThrowing = false;
    [SerializeField]
    private bool rangeEnemy;
    [SerializeField]
    private bool normalEnemy;
    [Header("UI")]
    [SerializeField]
    private Sprite normalEnemyImage;
    [SerializeField]
    private Sprite rangeImage;
    [SerializeField]
    private Image mobIcon;

    [Header("NavMesh Agent")]
    [SerializeField]
    private NavMeshAgent agent;       // Reference to the NavMeshAgent component
    private Animator animator;        // Reference to the Animator component
    private Collider parentCollider;  // Reference to the Collider of the parent object
    private SliderBar sliderBar;

    protected virtual void Awake()
    {
        //Get components from object
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
    }
    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    protected virtual void Update()
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
                        if (rangeEnemy)
                        {
                            ThrowProjectile();  // Ranged attack
                        }
                        else
                        {
                            AttackPlayer();  // Melee attack
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
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            animator.SetTrigger("Death");
            Destroy(gameObject, 2f);
        }
    }

    protected virtual void EnemyAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log(currentHP);
    }

    public virtual int awardEXP()
    {
        return expPoints;
    }

    public float getHealth()
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
}*/