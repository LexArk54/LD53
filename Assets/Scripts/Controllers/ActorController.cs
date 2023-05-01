using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour {

    public Actor actor;

    public bool isPaused { get; protected set; }

    public virtual void Awake() {
        actor = GetComponent<Actor>();
        actor.Init(this);
    }

    public virtual void ResetObject() {
        actor.Init(this);
    }

    public void Pause() {
        isPaused = true;
        actor.isUnderControl = false;
        actor.movement.Stop();
    }

    public void Play() {
        isPaused = false;
        actor.isUnderControl = true;
        actor.SetIsDead(false);
    }

}
