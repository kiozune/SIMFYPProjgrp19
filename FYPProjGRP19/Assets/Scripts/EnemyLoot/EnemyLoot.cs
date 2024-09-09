using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{

    public GameObject EnemyDropModel;
    public int health = 100;
    public Transform tansform;


    if(health<=0)
    {
        Destroy(GameObject); // destroy enemy game object
        DropEnemyLoot(); //drop enemy loot

    }

    void DropEnemyLoot() // Enemy loot will drop when enemy is killed
    {
        Vector3 position = tansform.position; //store current position of enemy
        GameObject loot = Instantiate(EnemyDropModel, position+ new Vector3(0.0f,1.0f,0.0f),Quaternion.identity); // spawn the loot drop when enmy dies
        loot.setAcctive(true); //Set the loot object to active
        Destroy(loot,6f) //Destroy loot drop after 6 seconds

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
