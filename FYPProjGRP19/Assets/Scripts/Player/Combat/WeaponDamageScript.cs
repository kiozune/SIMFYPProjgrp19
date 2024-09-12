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

    // When an enemy enters the collider
    private void OnTriggerEnter(Collider other)
    {
        if (ifHitting)
        {
            // Check if the object is an enemy (you can check by tag or a specific component)
            if (other.tag == ("BasicEnemy"))
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
        if (other.tag == ("BasicEnemy"))
        {
            // Remove enemy from the list
            enemiesInRange.Remove(other.gameObject);
        }
    }

    private void ApplyDamage(GameObject enemy)
    {
        EnemyAI healthScript = enemy.GetComponent<EnemyAI>();
        if (healthScript != null)
        {
            healthScript.TakeDamage(damageValue);
        }
        if (healthScript.returnHealthValue() <= 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            player.GetComponent<PlayerLevel>().AddEXP(enemy.gameObject.GetComponent<EnemyAI>().awardEXP());
            player.GetComponent<PlayerLevel>().UpdateXPSlider();
            enemiesInRange.Remove(enemy.gameObject);
            
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
}
