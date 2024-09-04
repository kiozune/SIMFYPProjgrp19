using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private GameObject gPauseMenu, // reference to the pause menu
        gConfirmation, // reference to the confirmation canvas
        gDimmedBG; // reference to canvas
    private int iBtnPressed; // to be able to reuse the confirmation screen
    [SerializeField] private TextMeshProUGUI confirmationHeader;

    private void Awake()
    {
        gPauseMenu = GameObject.Find("Menu Background");
        gConfirmation = GameObject.Find("Confirmation Prompt");
        gDimmedBG = GameObject.Find("Dimmed Background");
    }

    private void Start()
    {
        gPauseMenu.SetActive(false); // hide canvas from player 
        gConfirmation.SetActive(false); // hide confirmation prompt
        gDimmedBG.SetActive(false); // hide dimmed background
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gPauseMenu.SetActive(!gPauseMenu.activeInHierarchy);
            gDimmedBG.SetActive(!gDimmedBG.activeInHierarchy);

            if (Time.timeScale == 1.0f)
                Time.timeScale = 0f;
            else Time.timeScale = 1.0f;
        }
    }  

    public void Resume()
    {
        Time.timeScale = 1.0f;
        gDimmedBG.SetActive(false);
    }

    private void ConfirmationPrompt(string prompt)
    { 
        confirmationHeader.text = "ARE YOU SURE YOU WOULD LIKE TO "
            + prompt.ToUpper() + "?";
    }

    public void Confirm()
    {
        // player presses yes on the confirmation pop-up
        switch (iBtnPressed)
        {
            case 0: // restart scene/level
                Resume();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload scene
                break;
            case 1: // return to hub
                Resume();
                SceneManager.LoadScene(""); // load scene named here
                break;
            case 2: // main menu
                Resume();
                SceneManager.LoadScene("MainMenu"); // load scene named MainMenu
                break;
            default: // quit game
                Application.Quit();
                // for use in editor
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                break;
        }
    } 

    public void RestartScene()
    {
        iBtnPressed = 0;
        ConfirmationPrompt("RESTART THE LEVEL");
    }

    public void ReturnToHub()
    {
        iBtnPressed = 1;
        ConfirmationPrompt("RETURN TO HUB");
    }

    public void MainMenu()
    {
        iBtnPressed = 2;
        ConfirmationPrompt("RETURN MAIN MENU");
    }

    public void QuitGame()
    {
        iBtnPressed = 3;
        ConfirmationPrompt("QUIT GAME");
    }
}
