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
    private Animator robotAnimator;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject[] playButtonList;

    [SerializeField]
    private GameObject[] mainMenuBtnList;

    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioClip[] audioClipList;

    // Start is called before the first frame update
    void Start()
    {
        robotAnimator = robotModel.GetComponent<Animator>();
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
        //Delete previous player pref settings first
        PlayerPrefs.DeleteKey("meleeWeapon");
        PlayerPrefs.DeleteKey("rangeWeapon");
        sfxSource.Play();
        //Melee wep check if user selects melee it will set melee to true and range to false
        PlayerPrefs.SetInt("Meleewep", 1);
        PlayerPrefs.SetInt("RangeWep", 0);

        SceneManager.LoadScene("First Level");
    }
    public void onRangeClicked()
    {

        PlayerPrefs.DeleteKey("meleeWeapon");
        PlayerPrefs.DeleteKey("rangeWeapon");
        sfxSource.Play();
        PlayerPrefs.SetInt("Meleewep", 0);
        PlayerPrefs.SetInt("Rangewep", 1);

        SceneManager.LoadScene("First Level");
    }

    public void exitGame()
    {
        exitGame();
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
