using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class PirateColliderAttack : MonoBehaviour
{
    private float damage;

    private void Start()
    {
        damage = gameObject.GetComponentInParent<PirateEnemyAI>().attackDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHP = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHP != null) playerHP.takeDamage(damage);
    }
}
