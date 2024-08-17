using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float powerUp = 3.0f;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
       player = GameObject.FindWithTag("Player"); // it will find the player tag
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        RelativeMovement rm = player.GetComponent<RelativeMovement>(); // check player if there movement
        rm.moveSpeed += powerUp;
        Destroy(this.gameObject);
    }
}
