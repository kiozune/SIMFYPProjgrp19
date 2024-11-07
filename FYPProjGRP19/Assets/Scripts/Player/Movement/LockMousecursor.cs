using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockMousecursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // Hide the cursor
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
