using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
   [Header("Player Prefabs")]
   [SerializeField]
    private GameObject player1Prefab;
    [SerializeField]
    private GameObject player2Prefab;
    [Header("Spawn Point Position")]
    [SerializeField]
    private Transform spawnPoint;

    void Start()
        {
            int isMultiplayer = PlayerPrefs.GetInt("Multiplayer", 0); // Default to singleplayer if not set

            if (isMultiplayer == 1)
            {
                // Spawn two players for multiplayer
                Instantiate(player1Prefab, new Vector3(-2 + spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity);
                Instantiate(player2Prefab, new Vector3(2 + spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity);
            }
            else
            {
                // Spawn one player for singleplayer
                Instantiate(player1Prefab, new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity);
            }
        }
}