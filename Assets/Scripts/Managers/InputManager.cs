using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

#if UNITY_EDITOR
    [ReadOnly] public InputActionAsset _settings;
#endif

    private static InputSettings maps;
    public static InputSettings.PlayerActions Game => maps.Player;
    public static InputSettings.UIActions UI => maps.UI;

    public static ControlMode mode = ControlMode.Game;

    public static float mouseSens;

    public enum ControlMode {
        Game,
        UI,
    }

    public static void SaveSettings() {
        PlayerPrefs.SetFloat("mouseSens", mouseSens);
    }

    public static void LoadSettings() {
        mouseSens = PlayerPrefs.GetFloat("mouseSens", 0.15f);
    }

    public void SetSens(Slider slider) {
        mouseSens = slider.value;
    }

    private void Awake() {
        maps = new InputSettings();
    }

    private void OnEnable() {
        if (mode == ControlMode.Game) {
            ActivateGameMode();
        } else {
            ActivateUIMode();
        }
    }

    private void OnDisable() {
        maps.Disable();
    }


    public static void ActivateUIMode() {
        maps.UI.Enable();
        maps.Player.Disable();
        Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = false;
    }

    public static void ActivateGameMode() {
        maps.Player.Enable();
        maps.UI.Disable();
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = true;
    }

}
