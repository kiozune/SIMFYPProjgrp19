using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrops : MonoBehaviour
{
    // Overall drop chance (percentage that anything drops)
    [Range(0, 100)]
    public float overallDropChance = 50f;

    [System.Serializable]
    public class Loot
    {
        public GameObject item;   // The item to drop
        [Range(0, 100)]
        public float dropChance;  // The chance of this specific item dropping
    }

    public Loot[] lootTable;  // Array of loot items and their chances

    // Method to handle dropping loot
    public void DropLoot()
    {
        // First, check if any loot should drop at all
        float roll = Random.Range(0f, 100f);
        if (roll > overallDropChance)
        {
            Debug.Log("No loot dropped.");
            return; // Exit if no loot should drop
        }

        // Now, determine which specific loot item to drop
        foreach (Loot loot in lootTable)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= loot.dropChance)
            {
                Instantiate(loot.item, transform.position, Quaternion.identity);
                Debug.Log($"Dropped: {loot.item.name}");
                return; // Only drop one item, then exit the function
            }
        }

        Debug.Log("No specific item was dropped.");
    }
}