/*using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : EnemyAI
{
    [Header("Enemy Variables")]
    [SerializeField]
    public float meleeDamage = 20f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        maxHP = 100f;
        currentHP = maxHP;
        expPoints = 100;
    }

    protected override void EnemyAttack()
    {
        base.EnemyAttack();

        PlayerHealth playerHP = playerTransform.gameObject.GetComponent<PlayerHealth>();
        if (playerHP != null)
        {
            playerHP.takeDamage(meleeDamage);
        }
    }
}
*/