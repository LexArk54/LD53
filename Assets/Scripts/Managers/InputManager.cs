using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : Manager {

#if UNITY_EDITOR
    [ReadOnly] public InputActionAsset _settings;
#endif

    private static InputSettings maps;
    public static InputSettings.PlayerActions Game => maps.Player;
    public static InputSettings.UIActions UI => maps.UI;

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

    public override void Init() {
        base.Init();
        if (maps != null) return;
        maps = new InputSettings();
    }

    private void OnDisable() {
        maps.Disable();
    }


    public static void UpdateMode () {
        if (UIManager.main.HasActiveElement()) {
            ActivateUIMode();
        } else {
            ActivateGameMode();
        }
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
