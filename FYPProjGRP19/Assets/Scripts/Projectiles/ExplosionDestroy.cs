using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestroy : MonoBehaviour
{
    [SerializeField]
    private float currTime = 0;
    [SerializeField]
    private float maxTime = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;

        if (currTime > maxTime)
        {
            Destroy(this.gameObject);
        }
    }
}
