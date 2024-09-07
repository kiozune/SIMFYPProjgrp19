using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;         // Reference to the player's transform
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
    }
}
