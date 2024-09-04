using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{  
    private int iBtnPressed; // to be able to reuse the confirmation screen  
    [SerializeField] private TextMeshProUGUI confirmationHeader;

    [Header("Menus and Sub-Menus")]
    [SerializeField] private GameObject gDimmedBG;
    [SerializeField] private GameObject gPauseMenu;
    [SerializeField] private GameObject gConfirmation;
    [Header("Settings")]
    [SerializeField] private GameObject gSettingsMenu;
    [SerializeField] private GameObject gGraphicsMenu;
    [SerializeField] private GameObject gAudioMenu;

    private void Awake()
    {
        if (gPauseMenu == null)
            gPauseMenu = GameObject.Find("Menu Background");
        if (gConfirmation == null)
            gConfirmation = GameObject.Find("Confirmation Prompt");
        if (gDimmedBG == null)
            gDimmedBG = GameObject.Find("Dimmed Background");
        if (gSettingsMenu == null)
            gSettingsMenu = GameObject.Find("Settings");
    }

    private void Start()
    {
        gPauseMenu.SetActive(false); // hide canvas from player 
        gConfirmation.SetActive(false); // hide confirmation prompt
        gDimmedBG.SetActive(false); // hide dimmed background
        gSettingsMenu.SetActive(false); // hide settings menu
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gPauseMenu.SetActive(!gPauseMenu.activeInHierarchy);
            if (!gPauseMenu.activeInHierarchy) // make dimmed background and time scale dependent on pause menu
            {
                gDimmedBG.SetActive(false);
                Time.timeScale = 1.0f;
            }
            else 
            {
                gDimmedBG.SetActive(true); 
                Time.timeScale = 0f;
            } 
            gSettingsMenu.SetActive(false); // ensure settigns menu closes 
        }
    }  

    public void OpenSettingsMenu()
    {
        gPauseMenu.SetActive(false);
        gSettingsMenu.SetActive(true);
        gGraphicsMenu.SetActive(false);
        gAudioMenu.SetActive(false);
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
                // currently not in use - main menu acts as the hub
                // this section runs on the assumption that the hub is a separate scene that the player can traverse around with their character
                Resume();
                SceneManager.LoadScene("Main Menu"); // load scene named here
                break;
            case 2: // main menu
                Resume();
                SceneManager.LoadScene("Main Menu"); // load scene named MainMenu
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
