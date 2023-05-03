using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : CharacterComponent {

    [SerializeField] private AudioSource headAudioSource;
    [SerializeField] private AudioSource footAudioSource;

    [SerializeField] private AudioClipPreset[] stepClips;

    [SerializeField] private AudioClipPreset[] jumpClips;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Material material;

    public void PlayStepSound() {
        if (!character.movement.isGrounded) return;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, layerMask)) {
            footAudioSource.clip = stepClips[Random.Range(0, stepClips.Length)].clip;
            footAudioSource.volume = stepClips[Random.Range(0, stepClips.Length)].volume;
        } else {
            footAudioSource.clip = stepClips[Random.Range(0, stepClips.Length)].clip;
            footAudioSource.volume = stepClips[Random.Range(0, stepClips.Length)].volume;
        }
        footAudioSource.pitch = Random.Range(0.8f, 1.1f);
        footAudioSource.Play();
    }

    public void PlayJumpSound() {
        footAudioSource.clip = jumpClips[Random.Range(0, jumpClips.Length)].clip;
        footAudioSource.volume = jumpClips[Random.Range(0, jumpClips.Length)].volume;
        footAudioSource.Play();
    }

    public void PlayTalkSound() {
        headAudioSource.Play();
    }

}
