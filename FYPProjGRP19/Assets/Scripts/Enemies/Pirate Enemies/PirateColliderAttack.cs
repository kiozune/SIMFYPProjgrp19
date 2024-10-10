using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class PirateColliderAttack : MonoBehaviour
{ 
    [SerializeField] private bool isFromBasicEnemy; 
    [SerializeField] private bool isFromBoss; 
    private float damage;

    private void Start()
    {
        if (!isFromBasicEnemy && !isFromBoss) 
            Debug.LogError("[" + name + "] A bool value hasn't been set");
        else
        {
            if (isFromBasicEnemy) damage = gameObject.GetComponentInParent<PirateEnemyAI>().attackDamage;
            else if (isFromBoss) damage = gameObject.GetComponentInParent<PirateBossAI>().attackDamage; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHP = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHP != null) playerHP.takeDamage(damage);
    }
}
