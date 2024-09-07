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
    public Camera mainCamera;               // Reference to the main camera
    public float cameraBufferDistance = 5f; // Additional buffer distance outside camera view
    public float maxSpawnDistance = 30f;    // Maximum distance beyond the camera for spawning
    public float minSpawnDistance = 5f;     // Minimum distance just outside the camera for spawning

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
        // Calculate the camera frustum planes (the viewable area of the camera)
        Plane[] cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        // Loop through all child obstacles of this GameObject
        foreach (Transform obstacle in transform)
        {
            if (currentEnemyCount >= maxEnemiesToSpawn)
                return; // Stop spawning if max number of enemies is reached

            Vector3 spawnPosition = Vector3.zero; // Initialize spawnPosition with a default value
            bool validPositionFound = false;

            // Try to find a valid spawn point along the bounds of this obstacle and outside camera view
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

                // Check if the random point is outside the camera view and within the required distance
                float distanceFromCamera = Vector3.Distance(mainCamera.transform.position, randomPointInBounds);
                if (!IsInCameraView(cameraFrustumPlanes, randomPointInBounds) &&
                    distanceFromCamera >= minSpawnDistance &&
                    distanceFromCamera <= maxSpawnDistance)
                {
                    // Find a valid position within the NavMesh near the random point
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPointInBounds, out hit, navMeshCheckRadius, NavMesh.AllAreas))
                    {
                        spawnPosition = hit.position;
                        validPositionFound = true;
                        break;
                    }
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
                Debug.Log("Failed to find valid spawn point outside camera view near obstacle: " + obstacle.name);
            }
        }
    }

    // Helper function to check if a position is within the camera's view
    bool IsInCameraView(Plane[] cameraFrustumPlanes, Vector3 position)
    {
        // Add a buffer distance to ensure the enemy is slightly outside the camera's view
        Vector3 bufferedPosition = position + (position - mainCamera.transform.position).normalized * cameraBufferDistance;

        // Check if the buffered position is inside the camera's frustum
        return GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, new Bounds(bufferedPosition, Vector3.one));
    }
}
