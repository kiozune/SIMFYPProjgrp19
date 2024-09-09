using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;         // Reference to the player's transform
    public float hp = 100f;          // Enemy health points
    public float damageFromProjectile = 20f; // Amount of damage taken from each projectile hit

    private NavMeshAgent agent;      // Reference to the NavMeshAgent component

    void Start()
    {
        // Get the NavMeshAgent component attached to this enemy
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Continuously set the player's position as the destination for pathfinding
        if (player != null)
        {
            agent.SetDestination(player.position);
        }

        // If HP drops to 0 or below, destroy the enemy
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    // This function is called when the enemy is hit by a projectile
    public void TakeDamage(float damage)
    {
        hp -= damage;  // Reduce the enemy's HP by the damage amount

        // Optional: Display the current HP for debugging purposes
        Debug.Log("Enemy HP: " + hp);

        // If HP reaches 0 or below, destroy the enemy
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Example trigger or collision detection for the projectile
    void OnTriggerEnter(Collider other)
    {
        // Check if the enemy was hit by a projectile
        if (other.CompareTag("Projectile"))
        {
            // Take damage when hit by a projectile
            TakeDamage(damageFromProjectile);

            // Destroy the projectile after it hits the enemy
            Destroy(other.gameObject);
        }
    }
}
