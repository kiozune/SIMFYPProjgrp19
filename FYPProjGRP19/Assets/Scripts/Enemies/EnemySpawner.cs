using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyData
{
    public GameObject enemyPrefab;           // The enemy prefab to spawn
    public int minRange;                     // Minimum dice roll number for this enemy
    public int maxRange;                     // Maximum dice roll number for this enemy
    public bool isUnique;                    // Whether this enemy type is unique (can't spawn consecutively)
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Input")]
    public List<EnemyData> enemies;          // List of enemies with their associated numbers
    public Transform player;                 // Reference to the player
    public Camera mainCamera;                // Reference to the main camera

    [Header("Enemy Spawn Conditions")]
    public float navMeshCheckRadius = 2f;    // Radius to check for valid NavMesh position
    public int maxEnemiesToSpawn = 50;       // Maximum number of enemies to spawn
    public int maxAttemptsPerEnemy = 10;     // Max number of attempts to find a valid spawn point for each enemy
    public float cameraBufferDistance = 5f;  // Additional buffer distance outside camera view
    public float maxSpawnDistance = 30f;     // Maximum distance beyond the camera for spawning
    public float minSpawnDistance = 5f;      // Minimum distance just outside the camera for spawning

    [Header("Enemy Spawn Rate/Difficulty")]
    public float initialSpawnInterval = 5f;  // Initial time interval between enemy spawns
    public float spawnRateIncreaseInterval = 10f; // How often the spawn rate increases
    public float spawnRateDecreaseFactor = 0.9f;  // Factor to decrease the spawn interval
    public float minSpawnInterval = 1f;      // Minimum possible spawn interval to avoid overwhelming the player

    private int currentEnemyCount = 0;       // Tracks the current number of spawned enemies
    private float currentSpawnInterval;      // Tracks the current time interval between spawns
    private float elapsedTime = 0f;          // Tracks how long the game has been running
    private float lastSpawnRateIncreaseTime = 0f; // Tracks when the spawn rate was last increased
    private EnemyData lastSpawnedEnemy = null; // Reference to the last spawned enemy

    void Start()
    {
        // Set the initial spawn interval
        currentSpawnInterval = initialSpawnInterval;

        // Start the coroutine to spawn enemies over time
        StartCoroutine(SpawnEnemiesOverTime());
    }

    IEnumerator SpawnEnemiesOverTime()
    {
        // Continue spawning enemies until max count is reached
        while (currentEnemyCount < maxEnemiesToSpawn)
        {
            // Roll the dice and determine which enemy to spawn
            int diceRoll = Random.Range(1, 101);  // 100-sided dice roll (1 to 100)
            EnemyData enemyToSpawn = GetEnemyFromDiceRoll(diceRoll);

            // Prevent consecutive spawning of unique enemies
            if (enemyToSpawn != null && (enemyToSpawn.isUnique && enemyToSpawn == lastSpawnedEnemy))
            {
                Debug.Log("Skipping consecutive spawn of unique enemy: " + enemyToSpawn.enemyPrefab.name);
            }
            else
            {
                // Spawn the enemy if conditions are met
                if (enemyToSpawn != null)
                {
                    SpawnEnemy(enemyToSpawn);
                    lastSpawnedEnemy = enemyToSpawn;  // Track the last spawned enemy
                }
            }

            // Wait for the current spawn interval before spawning the next enemy
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    void Update()
    {
        // Track the elapsed time
        elapsedTime += Time.deltaTime;

        // If enough time has passed, increase the difficulty by decreasing the spawn interval
        if (elapsedTime - lastSpawnRateIncreaseTime >= spawnRateIncreaseInterval)
        {
            // Decrease the spawn interval, but don't go below the minimum
            currentSpawnInterval = Mathf.Max(currentSpawnInterval * spawnRateDecreaseFactor, minSpawnInterval);

            // Update the time when the spawn rate was last increased
            lastSpawnRateIncreaseTime = elapsedTime;
        }
    }

    // Function to roll the dice and determine which enemy to spawn
    EnemyData GetEnemyFromDiceRoll(int diceRoll)
    {
        foreach (EnemyData enemy in enemies)
        {
            // Check if the dice roll falls within the min and max range for this enemy
            if (diceRoll >= enemy.minRange && diceRoll <= enemy.maxRange)
            {
                return enemy;
            }
        }
        return null;  // No enemy found for this dice roll
    }

    // Function to spawn a specific enemy
    void SpawnEnemy(EnemyData enemyData)
    {
        // Find a valid spawn point near obstacles or based on your existing logic
        Vector3 spawnPosition = FindValidSpawnPosition();

        if (spawnPosition != Vector3.zero)
        {
            // Spawn the enemy at the valid position
            GameObject enemy = Instantiate(enemyData.enemyPrefab, spawnPosition, Quaternion.identity);

            // Assign the player reference to the EnemyAI script
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.player = player;
            }

            currentEnemyCount++; // Increment the enemy count
        }
    }

    // Custom function to find a valid spawn position (based on your existing logic)
    Vector3 FindValidSpawnPosition()
    {
        // Calculate the camera frustum planes (the viewable area of the camera)
        Plane[] cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        foreach (Transform obstacle in transform)
        {
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
                    obstacle.position.y,  // Keep the Y position fixed
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
                return spawnPosition;
            }
        }

        return Vector3.zero; // No valid position found
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
