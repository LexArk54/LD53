using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour {

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider fxVolumeSlider;
    public Slider mouseSensivity;


    private void OnEnable() {
        masterVolumeSlider.value = AudioManager.main.masterVolume;
        musicVolumeSlider.value = AudioManager.main.musicVolume;
        fxVolumeSlider.value = AudioManager.main.fxVolume;
        mouseSensivity.value = InputManager.mouseSens;
        Time.timeScale = 0;
    }

    private void OnDisable() {
        AudioManager.main.SaveSettings();
        InputManager.SaveSettings();
        Time.timeScale = 1;
    }
    public void SetMasterVolume(Slider slider) => AudioManager.main.masterVolume = slider.value;
    public void SetMusicVolume(Slider slider) => AudioManager.main.musicVolume = slider.value;
    public void SetFXVolume(Slider slider) => AudioManager.main.fxVolume = slider.value;
    public void SetMouseSensivity(Slider slider) => InputManager.mouseSens = slider.value;

}