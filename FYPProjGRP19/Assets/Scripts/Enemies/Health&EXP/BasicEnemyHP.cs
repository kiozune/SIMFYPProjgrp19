using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyHP : MonoBehaviour
{
    [Header("Enemy Values")] 

    [SerializeField]
    [Tooltip("Enemy's health before destroying it and awarding HP")]
    private float Health = 100f;
    [SerializeField]
    [Tooltip("Award EXP to the players on death")]
    private int experiencePoints = 35;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void takeDamage(float damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Destroy(this.gameObject);
        }
    }
    public bool checkHealth()
    {
        if (Health < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public int awardEXP()
    {
        return experiencePoints;
    }
    //public void OnTriggerEnter(Collider other)
    //{
    //    other.gameObject.GetComponent<PlayerHealth>().takeDamage(500);
    //}
}
