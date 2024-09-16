using EnemyInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class projectileDamage : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Damage for the projectile to enemy")]
    private float damage = 55;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setDamage(float actualDamage)
    {
        damage = actualDamage;
    }
    public void OnTriggerEnter(Collider other)
    {
        // Try to get the IEnemy component from the collided object
        IEnemy enemy = other.GetComponent<IEnemy>();

        if (enemy != null)  // If the collided object is an enemy
        {
            // Apply damage to the enemy
            enemy.TakeDamage(damage);

            // Check if the enemy is defeated
            if (enemy.hp <= 0)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    PlayerLevel playerLevel = player.GetComponent<PlayerLevel>();
                    if (playerLevel != null)
                    {
                        playerLevel.AddEXP(enemy.expGained);
                        playerLevel.UpdateXPSlider();
                    }
                }
            }
        }
    }
}
