using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gGameOverCanvas;

    private void Awake()
    {
        if (gGameOverCanvas == null)
            gGameOverCanvas = GameObject.Find("Game Over Canvas");
    }

    private void Start()
    {
        gGameOverCanvas.SetActive(false); // hide from player
    }

    /// <summary>
    /// Call this function when the player dies
    /// </summary>
    public void GameOverScreen()
    {
        Time.timeScale = 0f; // prevent objects from moving
        // need to add lines to prevent player movement

        gGameOverCanvas.SetActive(true);
    }

    private void Resume()
    {
        Time.timeScale = 1.0f;
        gGameOverCanvas.SetActive(false);
    }

    public void Retry()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload scene
    } 

    public void MainMenu()
    {
        Resume();
        SceneManager.LoadScene("Main Menu"); // load scene named MainMenu
    }

    public void QuitGame()
    {
        Application.Quit();
        // for use in editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
