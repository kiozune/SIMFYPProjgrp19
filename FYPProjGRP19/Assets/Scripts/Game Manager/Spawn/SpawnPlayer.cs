using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [Header("Player Prefabs")]
    [SerializeField]
    private GameObject KeyboardPrefab;
    [SerializeField]
    private GameObject ControllerPrefab;
    [Header("Spawn Point Position")]
    [SerializeField]
    private Transform spawnPoint;

    void Start()
    {
        {
            string inputType = PlayerPrefs.GetString("InputType", "Keyboard"); // Default to "Keyboard" if not set

            if (inputType == "Keyboard")
            {
                Debug.Log("Keyboard selected. Configuring for keyboard input.");
                Instantiate(KeyboardPrefab, new Vector3(-2 + spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity);
            }
            else if (inputType == "Controller")
            {
                Debug.Log("Controller selected. Configuring for controller input.");
                Instantiate(ControllerPrefab, new Vector3(2 + spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity);
            }
        }
    }
}