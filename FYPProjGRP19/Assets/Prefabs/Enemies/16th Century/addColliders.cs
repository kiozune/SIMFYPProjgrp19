using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class addColliders : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var allDescendents = GetComponentsInChildren<Transform>();
        foreach (var t in allDescendents)
        {
            t.AddComponent<BoxCollider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
