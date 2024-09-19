using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Damage for the projectile to enemy")]
    private float damage = 10f;


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
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().takeDamage(damage);
        }
    }
}
