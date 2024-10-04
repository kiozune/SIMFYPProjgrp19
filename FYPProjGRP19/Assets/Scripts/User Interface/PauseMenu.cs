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

    private void Awake() // in the event something isn't assigned
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
        gSettingsMenu.SetActive(false); // hide settigns menu
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Pause state changed"); 

            gPauseMenu.SetActive(!gPauseMenu.activeSelf);
            if (!gPauseMenu.activeSelf) // make dimmed background dependent on pause menu
                gDimmedBG.SetActive(false);
            else gDimmedBG.SetActive(true);
            gSettingsMenu.SetActive(false); // ensure settigns menu closes as well

            // time scale dependent on pause menu
            if (gPauseMenu.activeSelf)
                Time.timeScale = 0f;
            else Time.timeScale = 1.0f;
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
            case 1: // main menu
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

    public void MainMenu()
    {
        iBtnPressed = 1;
        ConfirmationPrompt("RETURN MAIN MENU");
    }

    public void QuitGame()
    {
        iBtnPressed = 2;
        ConfirmationPrompt("QUIT GAME");
    }
}
