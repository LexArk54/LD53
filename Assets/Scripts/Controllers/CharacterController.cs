using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public Character character;

    public bool isPaused { get; protected set; }

    public virtual void Awake() {
        character = GetComponent<Character>();
        character.Init(this);
    }

    public virtual void ResetObject() {
        character.Init(this);
    }

    public void Pause() {
        isPaused = true;
        character.isUnderControl = false;
        character.movement.Stop();
    }

    public void Play() {
        isPaused = false;
        character.isUnderControl = true;
        character.SetIsDead(false);
    }

}
