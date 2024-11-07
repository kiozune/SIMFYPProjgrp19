using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeElapsed : MonoBehaviour
{
    private float timeRemaining;
    private int minutes;
    private int seconds;
    [SerializeField]
    private TextMeshProUGUI timeText;
    [SerializeField]
    private string bossLevel;

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = 360f; // Set to 6 minutes (6 * 60 seconds)
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            minutes = Mathf.FloorToInt(timeRemaining / 60f);
            seconds = Mathf.FloorToInt(timeRemaining % 60f);

            timeText.text = "Time Remaining: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            loadMainMenu();
        }

        // Debug: Decrease time remaining quickly by pressing PageDown
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            timeRemaining = 1; // Set to 1 second to quickly reach zero
            Debug.Log(timeRemaining);
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
        SceneManager.LoadScene(bossLevel);
    }
}