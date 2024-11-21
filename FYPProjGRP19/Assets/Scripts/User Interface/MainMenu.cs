using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Model Value")]
    [SerializeField]
    private GameObject robotModel;
    [SerializeField]
    private GameObject robotStageSelect;
    [SerializeField]
    private Animator robotAnimator;
    [SerializeField]
    private Animator robotStageAnimator;

    [Header("Stage select Values")]
    [SerializeField]
    private bool isMelee = false;
    [SerializeField]
    private bool isRanged = false;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject[] playButtonList;

    [SerializeField]
    private GameObject[] mainMenuBtnList;

    [SerializeField]
    private GameObject[] stageSelectList;

    [SerializeField]
    private GameObject settingsButton;

    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioClip[] audioClipList;
    [SerializeField]
    private GameObject[] keyboardSelectBtn;
    [SerializeField]
    private GameObject backButton;

    // Start is called before the first frame update
    void Start()
    {
        robotAnimator = robotModel.GetComponent<Animator>();
        robotStageAnimator = robotStageSelect.GetComponent<Animator>();
        sfxSource.clip = audioClipList[0];
    }



    public void onPlayClicked()
    {
        for (int i = 0; i < keyboardSelectBtn.Length; ++i)
        {
            keyboardSelectBtn[i].SetActive(true);
        }
        backButton.SetActive(true);
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
            mainMenuBtnList[j].SetActive(false);
        }
    }
    public void onKeyboardSelect()
    {
        robotModel.SetActive(true);
        robotAnimator.SetTrigger("angry");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(true);
        }
        for (int i = 0; i < keyboardSelectBtn.Length; ++i)
        {
            keyboardSelectBtn[i].SetActive(false);
        }

        // Save the selection to PlayerPrefs
        PlayerPrefs.SetString("InputType", "Keyboard");
        PlayerPrefs.Save();
    }
    public void onControllerSelect()
    {
        robotModel.SetActive(true);
        robotAnimator.SetTrigger("angry");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(true);
        }
        for (int i = 0; i < keyboardSelectBtn.Length; ++i)
        {
            keyboardSelectBtn[i].SetActive(false);
        }

        // Save the selection to PlayerPrefs
        PlayerPrefs.SetString("InputType", "Controller");
        PlayerPrefs.Save();
    }


    public void onBackclicked()
    {
        robotModel.SetActive(false);
        robotAnimator.SetTrigger("normal");
        robotStageAnimator.SetTrigger("normal");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(false);
        }
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
            mainMenuBtnList[j].SetActive(true);
        }
        for (int i = 0; i < keyboardSelectBtn.Length; ++i)
        {
            keyboardSelectBtn[i].SetActive(false);
        }
    }

    public void onMeleeClicked()
    {
        isMelee = true;
        isRanged = false;

        sfxSource.Play();

        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(false);
        }
        for (int j = 0; j < stageSelectList.Length; ++j)
        {
            stageSelectList[j].SetActive(true);
        }
        robotStageSelect.SetActive(true);
        robotModel.SetActive(false);
        robotStageAnimator.SetTrigger("shuffleDance");
    }
    public void onRangeClicked()
    {
        isRanged = true;
        isMelee = false;

        sfxSource.Play();

        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(false);
        }
        for (int j = 0; j < stageSelectList.Length; ++j)
        {
            stageSelectList[j].SetActive(true);
        }
        robotStageSelect.SetActive(true);
        robotModel.SetActive(false);
        robotStageAnimator.SetTrigger("shuffleDance");
    }

    public void onAncientStageSelect()
    {
        int i = Random.Range(0, 2);
        if (isMelee)
        {
            PlayerPrefs.DeleteKey("Meleewep");
            PlayerPrefs.DeleteKey("Rangewep");
            PlayerPrefs.SetInt("Meleewep", 1);
            PlayerPrefs.SetInt("Rangewep", 0);
        }
        else if (isRanged)
        {
            PlayerPrefs.DeleteKey("Meleewep");
            PlayerPrefs.DeleteKey("Rangewep");
            PlayerPrefs.SetInt("Meleewep", 0);
            PlayerPrefs.SetInt("Rangewep", 1);
        }
        switch(i)
        {
            case 0:
                SceneManager.LoadScene("First Level");
                break;
            case 1:
                SceneManager.LoadScene("13th Century Singapore Map 2");
                break;
        }
        
    }
    public void fromStageSelectBack()
    {
        robotModel.SetActive(true);
        robotStageSelect.SetActive(false);
        robotAnimator.SetTrigger("angry");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(true);
        }
        for (int j = 0; j < stageSelectList.Length; ++j)
        {
            stageSelectList[j].SetActive(false);
        }
    }

    public void exitGame()
    {
        Application.Quit();
        // for use in editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void rangeHover()
    {
        robotAnimator.SetTrigger("ranged_idle");
    }
    public void meleeHover()
    {
        robotAnimator.SetTrigger("melee_idle");
    }
    public void backHover()
    {
        robotAnimator.SetTrigger("scared");
    }
    public void resetAnimation()
    {
        robotAnimator.SetTrigger("angry");
    }
    public void onSettingsClick()
    {
        settingsButton.SetActive(true);
        sfxSource.Play();
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
            if(j != 1)
            mainMenuBtnList[j].SetActive(false);
        }
    }
    public void onBacksettingsClicked()
    {
        sfxSource.Play();
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
                mainMenuBtnList[j].SetActive(true);
        }
    }
    public void onMuseumClicked()
    {
        SceneManager.LoadScene("Museum");
    }

    public void loadSeventeenCentury()
    {
        if (isMelee)
        {
            PlayerPrefs.DeleteKey("Meleewep");
            PlayerPrefs.DeleteKey("Rangewep");
            PlayerPrefs.SetInt("Meleewep", 1);
            PlayerPrefs.SetInt("Rangewep", 0);
        }
        else if (isRanged)
        {
            PlayerPrefs.DeleteKey("Meleewep");
            PlayerPrefs.DeleteKey("Rangewep");
            PlayerPrefs.SetInt("Meleewep", 0);
            PlayerPrefs.SetInt("Rangewep", 1);
        }
        SceneManager.LoadScene("Third Level");
    }
    public void loadFourteenCentury()
    {
        if (isMelee)
        {
            PlayerPrefs.DeleteKey("Meleewep");
            PlayerPrefs.DeleteKey("Rangewep");
            PlayerPrefs.SetInt("Meleewep", 1);
            PlayerPrefs.SetInt("Rangewep", 0);
        }
        else if (isRanged)
        {
            PlayerPrefs.DeleteKey("Meleewep");
            PlayerPrefs.DeleteKey("Rangewep");
            PlayerPrefs.SetInt("Meleewep", 0);
            PlayerPrefs.SetInt("Rangewep", 1);
        }
        SceneManager.LoadScene("16th Century");
    }
    public void loadEighteenCentury()
    {
        if (isMelee)
        {
            PlayerPrefs.DeleteKey("Meleewep");
            PlayerPrefs.DeleteKey("Rangewep");
            PlayerPrefs.SetInt("Meleewep", 1);
            PlayerPrefs.SetInt("Rangewep", 0);
        }
        else if (isRanged)
        {
            PlayerPrefs.DeleteKey("Meleewep");
            PlayerPrefs.DeleteKey("Rangewep");
            PlayerPrefs.SetInt("Meleewep", 0);
            PlayerPrefs.SetInt("Rangewep", 1);
        }
        SceneManager.LoadScene("Japanese Level");
    }
}
