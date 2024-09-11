using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;          // Reference to the player's transform
    public float hp = 100f;           // Enemy health points
    public float damageFromProjectile = 20f;  // Amount of damage taken from each projectile hit

    private NavMeshAgent agent;       // Reference to the NavMeshAgent component
    private Animator animator;        // Reference to the Animator component
    private Collider parentCollider;  // Reference to the Collider of the parent object

    void Start()
    {
        // Get the NavMeshAgent component attached to this enemy
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is not attached to the enemy.");
        }

        // Get the Animator component attached to this enemy
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is not attached to the enemy.");
        }

        // Get the Collider from the parent object (it should be attached to the same GameObject as this script)
        parentCollider = GetComponent<Collider>();
        if (parentCollider == null)
        {
            Debug.LogError("Parent object collider not found.");
        }

        // Check if the player is assigned
        if (player == null)
        {
            Debug.LogError("Player is not assigned in the EnemyAI script.");
        }
    }

    void Update()
    {
        if (player != null && agent != null)
        {
            agent.SetDestination(player.position);
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

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;  // Reduce the enemy's HP by the damage amount

        Debug.Log("Enemy HP: " + hp);

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            TakeDamage(damageFromProjectile);
            Destroy(other.gameObject);
        }
    }
}
