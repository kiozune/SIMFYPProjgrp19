using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;          // Prefab of the enemy to spawn
    public float navMeshCheckRadius = 2f;   // Radius to check for valid NavMesh position
    public Transform player;                // Reference to the player
    public int maxEnemiesToSpawn = 50;      // Maximum number of enemies to spawn
    public int maxAttemptsPerEnemy = 10;    // Max number of attempts to find a valid spawn point for each enemy
    public float spawnInterval = 2f;        // Time interval between spawning enemies

    private int currentEnemyCount = 0;      // Tracks the current number of spawned enemies

    void Start()
    {
        // Start the coroutine to spawn enemies over time
        StartCoroutine(SpawnEnemiesOverTime());
    }

    IEnumerator SpawnEnemiesOverTime()
    {
        // Continue spawning enemies until max count is reached
        while (currentEnemyCount < maxEnemiesToSpawn)
        {
            // Spawn a single enemy near one of the obstacles
            SpawnEnemyNearObstacles();

            // Wait for the spawn interval before spawning the next enemy
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemyNearObstacles()
    {
        // Loop through all child obstacles of this GameObject
        foreach (Transform obstacle in transform)
        {
            if (currentEnemyCount >= maxEnemiesToSpawn)
                return; // Stop spawning if max number of enemies is reached

            Vector3 spawnPosition = Vector3.zero; // Initialize spawnPosition with a default value
            bool validPositionFound = false;

            // Try to find a valid spawn point along the bounds of this obstacle
            for (int i = 0; i < maxAttemptsPerEnemy; i++)
            {
                // Get the bounds of the obstacle (works with both Colliders and Renderers)
                Bounds bounds;
                if (obstacle.TryGetComponent<Collider>(out Collider collider))
                {
                    bounds = collider.bounds;
                }
                else if (obstacle.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    bounds = renderer.bounds;
                }
                else
                {
                    Debug.LogWarning("No Collider or Renderer found on obstacle.");
                    continue;
                }

                // Generate a random position within the bounds of the obstacle
                Vector3 randomPointInBounds = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    obstacle.position.y,  // Keep the Y position fixed if needed, or randomize slightly for vertical surfaces
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                // Find a valid position within the NavMesh near the random point
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPointInBounds, out hit, navMeshCheckRadius, NavMesh.AllAreas))
                {
                    spawnPosition = hit.position;
                    validPositionFound = true;
                    break;
                }
            }

            if (validPositionFound)
            {
                // Spawn the enemy at the valid position
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // Assign the player reference to the EnemyAI script
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                enemyAI.player = player;

                currentEnemyCount++; // Increment the enemy count
            }
            else
            {
                Debug.Log("Failed to find valid spawn point on the NavMesh near obstacle: " + obstacle.name);
            }
        }
    }
}
