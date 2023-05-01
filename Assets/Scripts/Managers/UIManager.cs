using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager main;

    public UISettings settings;
    public Image fader;
    public float screenFadeSpeed = 0.01f;

    private void Awake() {
        main = this;
        ScreenShow();
    }

    public void TogglePause() {
        settings.gameObject.SetActive(!settings.gameObject.activeSelf);
    }

    public bool hasActiveElement() {
        return settings.gameObject.activeSelf;
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
            c.a = Mathf.MoveTowards(c.a, to, screenFadeSpeed);
            fader.color = c;
        }
        fader.gameObject.SetActive(false);
        callback?.Invoke();
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
