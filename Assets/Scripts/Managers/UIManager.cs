using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Manager {

    public static UIManager main;

    public UIMainMenu mainMenu;
    public UISettings settings;
    public Image fader;
    public float screenFadeSpeed = 0.01f;
    public UIMessage message;
    public GameObject congrat;

    public Text itemHint;
    public Text dropHint;

    [Header("Fish Indicator")]
    public GameObject eyeIndicatorContainer;
    public Image eyeIndicator;
    [Header("Fish Indicator")]
    public GameObject fishIndicatorContainer;
    public Image fishIndicator;
    public Text fishIndicatorCount;


    public Color[] indicatorColors;

    public override void Awake() {
        base.Awake();
        if (this.InitializeSingleton(ref main)) {
            ScreenShow();
        }
        SetItemHint("");
        SetDropHint("");
        SetEYE(IndicatorColor.None);
        SetFishCount(0);
        SetCongrat(false);
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            mainMenu.gameObject.SetActive(true);
        } else {
            mainMenu.gameObject.SetActive(false);
        }
        settings.gameObject.SetActive(false);
    }

    public void SetItemHint(string text) {
        itemHint.text = text;
    }

    public void SetDropHint(string text) {
        itemHint.text = text;
    }

    public void SetCongrat(bool isVisible) {
        if (congrat.activeSelf != isVisible) {
            congrat.SetActive(isVisible);
        }
    }

    public void SetFishColor(int color) {
        if (color == IndicatorColor.None) {
            SetFishCount(0);
        } else {
            fishIndicator.color = indicatorColors[color];
        }
    }
    public void SetFishCount(int count) {
        var isVisible = count != 0;
        if (fishIndicatorContainer.activeSelf != isVisible) {
            fishIndicatorContainer.SetActive(isVisible);
        }
        if (isVisible) {
            fishIndicatorCount.text = "x" + count;
        } else {
            SetFishColor(IndicatorColor.White);
        }
    }

    public void SetEYE(int color) {
        if (color == IndicatorColor.None) {
            eyeIndicatorContainer.gameObject.SetActive(false);
            return;
        }
        eyeIndicatorContainer.gameObject.SetActive(true);
        eyeIndicator.color = indicatorColors[color];
    }

    public void TogglePause() {
        settings.gameObject.SetActive(!settings.gameObject.activeSelf);
        InputManager.UpdateMode();
    }

    public bool HasActiveElement() {
        return settings.gameObject.activeSelf || mainMenu.gameObject.activeSelf;
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
        yield return new WaitForSecondsRealtime(1f);
        while (c.a != to) {
            yield return new WaitForSecondsRealtime(.01f); ;
            c.a = Mathf.MoveTowards(c.a, to, screenFadeSpeed);
            fader.color = c;
        }
        callback?.Invoke();
        if (to == 0f) fader.gameObject.SetActive(false);
    }

    /// <summary>
    /// 0 - after start delay
    /// 1 - after on target
    /// 2 - after freeze
    /// 3 - after comeback
    /// </summary>
    /// <param name="target"></param>
    /// <param name="startDelay"></param>
    /// <param name="flowDuration"></param>
    /// <param name="freezeDuration"></param>
    /// <param name="callback"></param>
    public void CameraFlow(Transform target, float startDelay, float flowDuration, float freezeDuration, Action<int> callback) {
        StartCoroutine(_CameraFlowIE(target, startDelay, flowDuration, freezeDuration, callback));
    }
    IEnumerator _CameraFlowIE(Transform target, float startDelay, float flowDuration, float freezeDuration, Action<int> callback) {
        var camTransform = Camera.main.transform;
        var sourcePos = camTransform.position;
        var sourceRot = camTransform.rotation;
        var timeSpeed = flowDuration / Vector3.Distance(sourcePos, target.position);
        var timer = 0f;
        yield return new WaitForSeconds(startDelay);
        callback?.Invoke(0);
        while (timer < flowDuration) {
            timer += Time.deltaTime;
            camTransform.position = Vector3.Lerp(sourcePos, target.position, timer);
            camTransform.rotation = Quaternion.Lerp(sourceRot, target.rotation, timer);
            yield return new WaitForEndOfFrame();
        }
        callback?.Invoke(1);
        yield return new WaitForSeconds(freezeDuration);
        callback?.Invoke(2);
        timer = 0f;
        while (timer < flowDuration) {
            timer += Time.deltaTime;
            camTransform.position = Vector3.Lerp(target.position, sourcePos, timer);
            camTransform.rotation = Quaternion.Lerp(target.rotation, sourceRot, timer);
            yield return new WaitForEndOfFrame();
        }
        callback?.Invoke(3);
    }

}
