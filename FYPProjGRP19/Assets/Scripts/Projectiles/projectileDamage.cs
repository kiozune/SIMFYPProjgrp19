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
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BasicEnemy"))
        {
            other.gameObject.GetComponent<EnemyAI>().TakeDamage(damage);
            if (other.GetComponent<EnemyAI>().checkHealth())
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                player.GetComponent<PlayerLevel>().AddEXP(other.gameObject.GetComponent<EnemyAI>().awardEXP());
                player.GetComponent<PlayerLevel>().UpdateXPSlider();
            }
        }
    }
}
