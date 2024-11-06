using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnDistanceFromWall = 2f; // How far from the wall to spawn
    public float spawnInterval = 2f; // Time between enemy spawns
    public int maxEnemies = 10; // Maximum number of enemies at a time
    public float spawnCheckRadius = 1f; // Radius to check for obstacles when spawning
    public LayerMask obstacleLayer; // Layer for obstacles

    private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> activeEnemies = new List<GameObject>(); // To keep track of currently active enemies

    void Start()
    {
        // Collect all child objects (the invisible walls/obstacles) as spawn points
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }

        // Start continuously spawning enemies
        StartCoroutine(SpawnEnemiesOverTime());
    }

    IEnumerator SpawnEnemiesOverTime()
    {
        while (true) // Infinite loop to keep spawning enemies
        {
            // Check if the number of enemies is less than the maximum
            if (activeEnemies.Count < maxEnemies)
            {
                // Pick a random spawn point (random invisible wall)
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

                // Find a valid position near the wall to spawn the enemy
                Vector3 spawnPos = GetValidSpawnPosition(spawnPoint);

                if (spawnPos != Vector3.zero) // If a valid position is found
                {
                    // Spawn the enemy at the generated position
                    GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                    // Add the new enemy to the list of active enemies
                    activeEnemies.Add(newEnemy);

                    // Destroy the enemy after 5 seconds and remove it from the list
                    Destroy(newEnemy, 5f);
                    StartCoroutine(RemoveEnemyFromListAfterDelay(newEnemy, 5f));
                }
            }

            // Wait for the specified interval before checking again
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator RemoveEnemyFromListAfterDelay(GameObject enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeEnemies.Remove(enemy); // Remove the enemy from the list after it is destroyed
    }

    Vector3 GetValidSpawnPosition(Transform wall)
    {
        Vector3 wallPos = wall.position;

        // Try several random positions around the wall until a valid one is found
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnDistanceFromWall, spawnDistanceFromWall),
                0,
                Random.Range(-spawnDistanceFromWall, spawnDistanceFromWall)
            );

            Vector3 spawnPos = wallPos + randomOffset;

            // Check if the spawn position overlaps with any obstacle
            if (!Physics.CheckSphere(spawnPos, spawnCheckRadius, obstacleLayer))
            {
                return spawnPos; // Return this position if no obstacles are detected
            }
        }

        Debug.LogWarning("Could not find a valid spawn position near the wall.");
        return Vector3.zero; // Return zero if no valid position is found after several attempts
    }
}
