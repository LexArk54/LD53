using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Manager {

    public static GameManager main;

    public Manager[] managers;

    public CrabController[] crabs;

    public override void Awake() {
        base.Awake();
        if (this.InitializeSingleton(ref main)) {
            RebuildManagers();
        }
    }

    public void RebuildManagers() {
        foreach (var manager in managers) {
            manager.Awake();
        }
        InputManager.UpdateMode();
    }

    void Start() {
        AudioManager.main.LoadSettings();
        InputManager.LoadSettings();
    }

}
