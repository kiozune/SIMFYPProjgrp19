using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private AudioSource sfxSource;
    [SerializeField]
    private AudioClip[] audioClipList;

    // Start is called before the first frame update
    void Start()
    {
        robotAnimator = robotModel.GetComponent<Animator>();
        robotStageAnimator = robotStageSelect.GetComponent<Animator>();
        sfxSource.clip = audioClipList[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onPlayClicked()
    {
        robotModel.SetActive(true);
        robotAnimator.SetTrigger("angry");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(true);
        }
        for(int j = 0; j < mainMenuBtnList.Length;++j)
        {
            mainMenuBtnList[j].SetActive(false);
        }
    }

    public void onBackclicked()
    {
        robotModel.SetActive(false);
        robotAnimator.SetTrigger("normal");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(false);
        }
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
            mainMenuBtnList[j].SetActive(true);
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
        robotStageAnimator.SetTrigger("happy");
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
        robotStageAnimator.SetTrigger("happy");
    }

    public void onAncientStageSelect()
    {
        if (isMelee)
        {
            //Delete previous player pref settings first
            PlayerPrefs.DeleteKey("Meleewep");
            PlayerPrefs.DeleteKey("Rangewep");
            //Melee wep check if user selects melee it will set melee to true and range to false
            PlayerPrefs.SetInt("Meleewep", 1);
            PlayerPrefs.SetInt("Rangewep", 0);
        }
        if(isRanged)
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
}
