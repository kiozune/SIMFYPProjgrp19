using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageScript : MonoBehaviour
{
    [Header("Damage Values")]
    [SerializeField]
    private float damageValue = 55f;
    [SerializeField]
    private bool ifHitting = false;
    [SerializeField]
    private List<GameObject> enemiesInRange = new List<GameObject>();

    [Header("Melee Settings")]
    [SerializeField]
    private BoxCollider meleeCollider;  // Reference to the weapon's melee collider
    [SerializeField]
    private float meleeRange = 2.0f;    // Range of the melee attack
    [SerializeField]
    private float meleeWidth = 1.0f;    // Width of the melee hitbox
    [SerializeField]
    private float meleeHeight = 1.0f;   // Height of the melee hitbox

    // Start is called before the first frame update
    void Start()
    {
        // If a melee collider exists, adjust its size
        if (meleeCollider != null)
        {
            AdjustMeleeRange();
        }
    }

    // When an enemy enters the collider
    private void OnTriggerEnter(Collider other)
    {
        if (ifHitting)
        {
            // Check if the object is an enemy (you can check by tag or a specific component)
            if (other.CompareTag("BasicEnemy"))
            {
                // Add enemy to the list
                enemiesInRange.Add(other.gameObject);
                Debug.Log("Hit!");
                // Apply damage to the enemy
                ApplyDamage(other.gameObject);
            }
        }
    }

    // When an enemy exits the collider
    private void OnTriggerExit(Collider other)
    {
        // Check if the object is an enemy
        if (other.CompareTag("BasicEnemy"))
        {
            // Remove enemy from the list
            enemiesInRange.Remove(other.gameObject);
        }
    }

    private void ApplyDamage(GameObject enemy)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(damageValue);
        }
        if (enemyAI.checkHealth())
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            player.GetComponent<PlayerLevel>().AddEXP(enemy.gameObject.GetComponent<EnemyAI>().awardEXP());
            player.GetComponent<PlayerLevel>().UpdateXPSlider();
            enemiesInRange.Remove(enemy);
        }
    }

    public bool AreAllEnemiesInRange(List<GameObject> allEnemies)
    {
        // Return true if all enemies are inside the collider
        return allEnemies.TrueForAll(enemy => enemiesInRange.Contains(enemy));
    }

    public bool returnIfHitting()
    {
        return ifHitting;
    }

    public void setBoolHit(bool isHit)
    {
        ifHitting = isHit;
    }

    public void upgradeDamage(float addDamage)
    {
        Debug.Log("Damage Upgraded");
        damageValue += addDamage;
    }

    // Method to adjust melee range
    public void AdjustMeleeRange()
    {
        if (meleeCollider != null)
        {
            // Adjust the size and center of the melee collider
            meleeCollider.size = new Vector3(meleeWidth, meleeHeight, meleeRange);
            meleeCollider.center = new Vector3(0, 1, meleeRange / 2);  // Move the hitbox forward
        }
    }
    public void IncreaseMeleeRange(float rangeValue)
    {
        meleeRange += rangeValue;
        AdjustMeleeRange();
    }
}