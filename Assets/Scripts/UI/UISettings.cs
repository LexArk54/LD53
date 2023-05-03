using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISettings : MonoBehaviour {

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider fxVolumeSlider;
    public Slider mouseSensivity;

    public GameObject restartLevel;


    private void OnEnable() {
        masterVolumeSlider.value = AudioManager.main.masterVolume;
        musicVolumeSlider.value = AudioManager.main.musicVolume;
        fxVolumeSlider.value = AudioManager.main.fxVolume;
        mouseSensivity.value = InputManager.mouseSens;
        Time.timeScale = 0;
        restartLevel.SetActive(SceneManager.GetActiveScene().name != "MainMenu");
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

    public void RestartLevel() {
        var scene = SceneManager.GetActiveScene();
        var loader = SceneManager.LoadSceneAsync(scene.name);
        loader.completed += (AsyncOperation obj) => {
            GameManager.main.RebuildManagers();
        };
    }
    public void BackToMenu() {
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            UIManager.main.TogglePause();
        } else {
            UIManager.main.ScreenFade(() => {
                var loader = SceneManager.LoadSceneAsync("MainMenu");
                loader.completed += (AsyncOperation obj) => {
                    GameManager.main.RebuildManagers();
                };
            });
        }
    }

}
