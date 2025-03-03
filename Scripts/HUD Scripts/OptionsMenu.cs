using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public AudioMixer sfxMixer;
    public AudioMixer musicMixer;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Resolution"))
        {
            UpdateResolution(PlayerPrefs.GetInt("Resolution"));
        }
    }

    public void UpdateResolution(int value)
    {
        switch (value)
        {
            case 0:
                PlayerPrefs.SetInt("Resolution", 0);
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;
            case 1:
                PlayerPrefs.SetInt("Resolution", 1);
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            default:
                break;
        }
    }
    //Dropdown Resolution

    //Music Slider
    public void ApplySettings()
    {
        UpdateResolution(resolutionDropdown.value);   
    }
    
}
