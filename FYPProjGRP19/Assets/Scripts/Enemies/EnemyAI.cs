using EnemyInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Transform playerTransform;
    public Transform player;
    //private SliderBar             //reference HP Bar here
    private IEnemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure that the object this AI is attached to implements IEnemy
        enemy = GetComponent<IEnemy>();

        if (enemy == null)
        {
            Debug.LogError("EnemyAI requires a component that implements IEnemy.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            transform.LookAt(playerTransform);
        }
    }

    public void setPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    public Transform getPlayer()
    {
        return playerTransform;
    }
    public void TakeDamage(float damage)
    {
        if (enemy != null)
        {
            // Pass the damage to the IEnemy implementation
            enemy.TakeDamage(damage);
        }
    }

    public float getHealth()
    {
        return enemy.hp;
    }

    public int getEXP()
    {
        return enemy.expGained;
    }

    public int awardEXP()
    {
        return experiencePoints;
    }
    private void changeMeleeIcon()
    {
        mobIcon.sprite = normalEnemyImage;

    }
    private void changeRangeIcon()
    {
        mobIcon.sprite = rangeImage;
    }
}
