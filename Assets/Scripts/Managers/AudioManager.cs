using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : Manager {

    public static AudioManager main;

    public AudioMixer mixer;

    public AudioClipPreset mainMusicPreset;
    private AudioClipPreset currentMusicPreset;
    [SerializeField] private AudioSource musicSource;
    Coroutine changeMusicCoroutine;

    public sealed class Name {
        public const string MasterVolume = "MasterVolume";
        public const string MasterPitch = "MasterPitch";
        public const string MusicVolume = "MusicVolume";
        public const string MusicPitch = "MusicPitch";
        public const string FXVolume = "FXVolume";
        public const string FXPitch = "FXPitch";
    }

    public void PlayMusic(AudioClipPreset preset, float changeDuration = 2f) {
        if (changeMusicCoroutine != null) {
            StopCoroutine(changeMusicCoroutine);
            changeMusicCoroutine = null;
        }
        float currentPresetVolume = 1f;
        if (currentMusicPreset) {
            currentPresetVolume = currentMusicPreset.volume;
        }
        currentMusicPreset = preset;
        changeMusicCoroutine = StartCoroutine(musicSource.FadeEnumerator(0, currentPresetVolume / changeDuration, () => { 
            musicSource.Stop();
            if (currentMusicPreset) {
                musicSource.clip = currentMusicPreset.clip;
                musicSource.Play();
                changeMusicCoroutine = StartCoroutine(musicSource.FadeEnumerator(currentMusicPreset.volume, currentMusicPreset.volume / changeDuration, () => {
                    changeMusicCoroutine = null;
                }));
            } else {
                changeMusicCoroutine = null;
            }
        }));
    }

    public float dbToNormalized(float db) {
        return Mathf.Exp(db / 20);
    }
    public float normaliedToDb(float value) {
        if (value <= 0) value = 0.0001f;
        return Mathf.Log(value) * 20;
    }

    public override void Init() {
        base.Init();
        main = this;
    }

    private void Start() {
        PlayMusic(mainMusicPreset);
    }

    public void SaveSettings() {
        PlayerPrefs.SetFloat(Name.MasterVolume, masterVolume);
        PlayerPrefs.SetFloat(Name.MusicVolume, musicVolume);
        PlayerPrefs.SetFloat(Name.FXVolume, fxVolume);
    }
    public void LoadSettings() {
        masterVolume = PlayerPrefs.GetFloat(Name.MasterVolume, .75f);
        musicVolume = PlayerPrefs.GetFloat(Name.MusicVolume, .5f);
        fxVolume = PlayerPrefs.GetFloat(Name.FXVolume, .5f);
    }

    public float masterVolume {
        get {
            mixer.GetFloat(Name.MasterVolume, out float value); 
            return dbToNormalized(value);
        }
        set => mixer.SetFloat(Name.MasterVolume, normaliedToDb(value));
    }
    public float masterPitch {
        get {
            mixer.GetFloat(Name.MasterPitch, out float value);
            return value;
        }
        set => mixer.SetFloat(Name.MasterPitch, value);
    }

    public float musicVolume {
        get {
            mixer.GetFloat(Name.MusicVolume, out float value);
            return dbToNormalized(value);
        }
        set => mixer.SetFloat(Name.MusicVolume, normaliedToDb(value));
    }
    public float musicPitch {
        get {
            mixer.GetFloat(Name.MusicPitch, out float value);
            return value;
        }
        set => mixer.SetFloat(Name.MusicPitch, value);
    }

    public float fxVolume {
        get {
            mixer.GetFloat(Name.FXVolume, out float value);
            return dbToNormalized(value);
        }
        set => mixer.SetFloat(Name.FXVolume, normaliedToDb(value));
    }
    public float fxPitch {
        get {
            mixer.GetFloat(Name.FXPitch, out float value);
            return value;
        }
        set => mixer.SetFloat(Name.FXPitch, value);
    }


}


public static class AudioSourceExtension {
    public static IEnumerator FadeEnumerator(this AudioSource source, float targetVolume, float fadeSpeed, Action callback = null) {
        fadeSpeed *= Time.fixedDeltaTime;
        while (source.volume != targetVolume) {
            source.volume = Mathf.MoveTowards(source.volume, targetVolume, fadeSpeed);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        callback?.Invoke();
    }
}