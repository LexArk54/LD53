using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour {

    public Character character;

    public bool isPaused { get; protected set; }

    public virtual void Awake() {
        character = GetComponent<Character>();
        character.Init(this);
    }

    public virtual void OnActorReseted() { }

    public void Pause() {
        isPaused = true;
        character.isUnderControl = false;
        character.movement.Stop();
    }

    public void Play() {
        isPaused = false;
        character.isUnderControl = true;
    }

    public virtual bool CanInteract(Character character) { return false; }
    public virtual void Interact(Character character) { }

}
