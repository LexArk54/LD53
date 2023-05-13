using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : MonoBehaviour {

    public GameObject[] levelBtns;

    private void Awake() {
        for (int i = 0; i < levelBtns.Length; i++) {
            levelBtns[i].SetActive(false);
        }
    }

    private void OnEnable() {
        var openedLevels = PlayerPrefs.GetInt("OpenedLevels", 0);
        if (openedLevels > levelBtns.Length) {
            openedLevels = 0;
        }
        for (int i = 0; i < openedLevels; i++) {
            levelBtns[i].SetActive(true);
        }
    }

    private void OnDisable() {
        for (int i = 0; i < levelBtns.Length; i++) {
            levelBtns[i].SetActive(false);
        }
    }

    public void PlayLevel(int level) {
        GameManager.ChangeScene("Level" + level);
    }

    public void Settings() {
        UIManager.main.TogglePause();
    }

}