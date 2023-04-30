using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager main;

    public UISettings settings;
    public UIInventory inventory;
    public Image fader;
    public float faderSpeed = 10f;

    private void Awake() {
        main = this;
        ScreenShow();
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

    public void ScreenShow(Action callback = null) {
        StartCoroutine(_ScreenFadeIE(1f, 0f, callback));
    }

    public void ScreenFade(Action callback = null) {
        StartCoroutine(_ScreenFadeIE(0f, 1f, callback));
    }

    IEnumerator _ScreenFadeIE(float from, float to, Action callback) {
        var c = fader.color;
        c.a = from;
        fader.color = c;
        fader.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        while (c.a != to) {
            yield return new WaitForFixedUpdate();
            c.a = Mathf.MoveTowards(c.a, to, faderSpeed);
            fader.color = c;
        }
        fader.gameObject.SetActive(false);
        callback?.Invoke();
    }

}
