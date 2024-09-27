using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GotoWIPScreen : MonoBehaviour
{
    private float transitionTime = 8f;
    private float currTime;

    // Start is called before the first frame update
    void Start()
    {
        currTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;

        if(currTime > transitionTime)
        {
            SceneManager.LoadScene("BossArena");
        }
    }
}
