using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeElapsed : MonoBehaviour
{
    private float timeElapsed;
    private int minutes;
    private int seconds;
    [SerializeField]
    private TextMeshProUGUI timeText;
    // Start is called before the first frame update
    void Start()
    {
        timeElapsed = 0f;
        minutes = 0;
        seconds = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;


        minutes = Mathf.FloorToInt(timeElapsed / 60f);
        seconds = Mathf.FloorToInt(timeElapsed % 60f);

        timeText.text = "Time Elapsed: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        if(Input.GetKeyDown(KeyCode.PageDown))
        {
            timeElapsed = 899;
            Debug.Log(timeElapsed);
        }
        if (minutes >= 15)
        {
            loadMainMenu();
        }
    }
    public int RetrieveSeconds()
    {
        return seconds;
    }
    public int RetrieveMinutes()
    {
        return minutes;
    }
    public void loadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
