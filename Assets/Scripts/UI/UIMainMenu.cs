using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour {

    public GameObject[] levelBtns;

    private void Awake() {
        for (int i = 0; i < levelBtns.Length; i++) {
            levelBtns[i].SetActive(false);
        }
    }

    private void OnEnable() {
        var openedLevels = PlayerPrefs.GetInt("OpenedLevels", -1);
        for (int i = 0; i <= openedLevels; i++) {
            levelBtns[i].SetActive(true);
        }
    }

    private void OnDisable() {
        for (int i = 0; i < levelBtns.Length; i++) {
            levelBtns[i].SetActive(false);
        }
    }

    public void Play() {
        UIManager.main.ScreenFade(() => {
            var loader = SceneManager.LoadSceneAsync("Level1");
            loader.completed += (AsyncOperation obj) => {
                GameManager.main.RebuildManagers();
            };
        });
    }

    public void PlayLevel(int level) {
        UIManager.main.ScreenFade(() => {
            var loader = SceneManager.LoadSceneAsync("Level" + level);
            loader.completed += (AsyncOperation obj) => {
                GameManager.main.RebuildManagers();
            };
        });
    }

    public void Settings() {
        UIManager.main.TogglePause();
    }

}