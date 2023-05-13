using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Manager {

    public static GameManager main;

    public Manager[] managers;

    public CrabController[] crabs;

    private void Awake() {
        if (this.InitializeSingleton(ref main)) {
            RebuildManagers();
        }
    }

    private void RebuildManagers() {
        foreach (var manager in managers) {
            manager.Init();
        }
        InputManager.UpdateMode();
    }

    void Start() {
        AudioManager.main.LoadSettings();
        InputManager.LoadSettings();
    }

    public static void ChangeScene(string sceneName, Action callback = null) {
        UIManager.main.ScreenFade(() => {
            var loader = SceneManager.LoadSceneAsync(sceneName);
            loader.completed += (AsyncOperation obj) => {
                main.RebuildManagers();
                callback?.Invoke();
            };
        });
    }

    public static Scene GetActiveScene() {
        return SceneManager.GetActiveScene();
    }

}
