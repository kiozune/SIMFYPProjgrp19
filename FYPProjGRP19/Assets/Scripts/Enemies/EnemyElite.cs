using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;

public class EnemyElite : EnemyAI
{
    [Header("Enemy Variables")]
    [SerializeField]
    public float meleeDamage = 50f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        maxHP = 200f;
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