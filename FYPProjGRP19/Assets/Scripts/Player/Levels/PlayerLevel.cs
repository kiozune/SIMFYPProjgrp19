using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevel : MonoBehaviour
{
    [Header("Level_Values")]
    [SerializeField]
    private int currentLevel = 1;
    [SerializeField]
    private int currentEXP = 0;
    [SerializeField]
    private int exptoLevel = 100;
    [SerializeField]
    private float expLevelMultiplier = 1.3f;
    [SerializeField]
    private GameObject vfxLevelUp;

    [Header("EXP Slider")]
    [SerializeField]
    private Slider xpSlider;

    // Start is called before the first frame update
    void Start()
    {
        UpdateXPSlider();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateXPSlider();
    }
    public void AddEXP(int EXP)
    {
        currentEXP += EXP;
        checkLevelUP();
    }
    private void checkLevelUP()
    {
        while (currentEXP >= exptoLevel)
        {
            levelUPCharacter();
        }
    }
    private void levelUPCharacter()
    {
        currentEXP -= exptoLevel;
        currentLevel++;
        exptoLevel = Mathf.RoundToInt(exptoLevel * expLevelMultiplier);
        GameObject levelUPVFX = Instantiate(vfxLevelUp, transform.position, Quaternion.identity);
    }

    public void UpdateXPSlider()
    {
        if (xpSlider != null)
        {
            xpSlider.value = (float)currentEXP / exptoLevel;  // Set slider value based on XP ratio
        }
    }
}