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
    [SerializeField]
    private bool isMultiplayer = false;

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
    private GameObject singleplayerButton;
    [SerializeField]
    private GameObject multiplayerButton;
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
        singleplayerButton.SetActive(true);
        multiplayerButton.SetActive(true);
        backButton.SetActive(true);
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
            mainMenuBtnList[j].SetActive(false);
        }
    }
    public void onSinglePlayerClicked()
    {
        robotModel.SetActive(true);
        robotAnimator.SetTrigger("angry");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(true);
        }
        singleplayerButton.SetActive(false);
        multiplayerButton.SetActive(false);
        isMultiplayer = false;
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
        singleplayerButton.SetActive(false);
        multiplayerButton.SetActive(false);
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

        SceneManager.LoadScene("First Level");
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
}
