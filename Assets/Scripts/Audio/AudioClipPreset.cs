using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "_Game/Audio/Clip Preset")]
public class AudioClipPreset : ScriptableObject {

    public AudioClip clip;
    [Range(0, 1)] public float volume = 1f;

}
