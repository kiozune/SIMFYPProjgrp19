using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    public GameObject EnemyDropModel;
    public float health = 100f;

    void Update()
    {
        // Check if health is below or equal to 0
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject); // destroy enemy game object
        DropEnemyLoot(); // drop enemy loot
    }

    void DropEnemyLoot() // Enemy loot will drop when enemy is killed
    {
        Vector3 position = transform.position; // store current position of enemy
        GameObject loot = Instantiate(EnemyDropModel, position + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity); // spawn the loot drop
        loot.SetActive(true); // Set the loot object to active
        Destroy(loot, 6f); // Destroy loot drop after 6 seconds
    }
}