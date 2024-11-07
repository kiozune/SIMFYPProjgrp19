using UnityEngine;
using System.Collections.Generic;

public class WeaponDamage2 : MonoBehaviour
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

    void Start()
    {
        if (meleeCollider != null)
        {
            AdjustMeleeRange();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ifHitting)
        {
            if (other.CompareTag("BasicEnemy"))
            {
                enemiesInRange.Add(other.gameObject);
                ApplyDamage(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BasicEnemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    private void ApplyDamage(GameObject enemy)
    {
        BossHealth bossHealthScript = enemy.GetComponent<BossHealth>();
        if (bossHealthScript != null)
        {
            bossHealthScript.TakeDamage(damageValue);
            if (bossHealthScript.currentHealth <= 0)
            {
                enemiesInRange.Remove(enemy);
            }
        }
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

    private void AdjustMeleeRange()
    {
        if (meleeCollider != null)
        {
            meleeCollider.size = new Vector3(meleeWidth, meleeHeight, meleeRange);
            meleeCollider.center = new Vector3(0, 1, meleeRange / 2);
        }
    }

    public void IncreaseMeleeRange(float rangeValue)
    {
        meleeRange += rangeValue;
        AdjustMeleeRange();
    }
}
