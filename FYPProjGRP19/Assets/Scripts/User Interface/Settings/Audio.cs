using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; 

public class Audio : MonoBehaviour
{
    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer masterMixer; 

    [Header("Parameter Names"), Tooltip("Ensure these match the exposed parameter names for each")]
    [SerializeField] private string masterParamName;
    [SerializeField] private string sfxParamName;
    [SerializeField] private string musicParamName;

    [Header("Audio Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        // In case sliders were not assigned in the Inspector
        if (masterSlider == null)
            masterSlider = GameObject.Find("Master Volume").GetComponent<Slider>();
        if (sfxSlider == null)
            sfxSlider = GameObject.Find("SFX").GetComponent<Slider>();
        if (musicSlider == null)
            musicSlider = GameObject.Find("Music").GetComponent<Slider>();

        // Master
        float savedVol = PlayerPrefs.GetFloat(masterParamName, masterSlider.maxValue / 2f); // Default to half volume if not found
        masterSlider.value = savedVol;
        SetMasterVol(savedVol);
        masterSlider.onValueChanged.AddListener(SetMasterVol);

        // SFX
        savedVol = PlayerPrefs.GetFloat(sfxParamName, sfxSlider.maxValue / 2f);
        sfxSlider.value = savedVol;
        SetSFXVol(savedVol);
        sfxSlider.onValueChanged.AddListener(SetSFXVol);

        // Music
        savedVol = PlayerPrefs.GetFloat(musicParamName, musicSlider.maxValue / 2f);
        musicSlider.value = savedVol;
        SetMusicVol(savedVol);
        musicSlider.onValueChanged.AddListener(SetMusicVol);
    }

    // Convert percentage fraction to decibels
    public float ConvertToDecibel(float _value)
    {
        return Mathf.Log10(Mathf.Max(_value, 0.0001f)) * 20f;
    }

    // Set volumes for each audio source group
    private void SetMasterVol(float _value)
    {
        masterMixer.SetFloat(masterParamName, ConvertToDecibel(_value / masterSlider.maxValue));
        PlayerPrefs.SetFloat(masterParamName, _value);
        PlayerPrefs.Save();  // Ensure saved immediately
    }

    private void SetSFXVol(float _value)
    {
        masterMixer.SetFloat(sfxParamName, ConvertToDecibel(_value / sfxSlider.maxValue));
        PlayerPrefs.SetFloat(sfxParamName, _value);
        PlayerPrefs.Save();
    }

    private void SetMusicVol(float _value)
    {
        masterMixer.SetFloat(musicParamName, ConvertToDecibel(_value / musicSlider.maxValue));
        PlayerPrefs.SetFloat(musicParamName, _value);
        PlayerPrefs.Save();
    }

}
