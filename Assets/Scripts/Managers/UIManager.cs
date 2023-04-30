using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager main;

    public UISettings settings;
    public UIInventory inventory;

    private void Awake() {
        main = this;
    }

    public void ToggleInventory() {
        inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
    }

    public void TogglePause() {
        settings.gameObject.SetActive(!settings.gameObject.activeSelf);
    }

    public bool hasActiveElement() {
        return inventory.gameObject.activeSelf || settings.gameObject.activeSelf;
    }


}
