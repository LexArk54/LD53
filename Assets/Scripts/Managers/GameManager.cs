using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager main;

    private void Awake() {
        main = this;
    }
    void Start() {
        AudioManager.main.LoadSettings();
        InputManager.LoadSettings();
    }

}
