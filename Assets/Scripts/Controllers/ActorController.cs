using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour {

    public Actor actor;

    public virtual void Awake() {
        actor = GetComponent<Actor>();
        actor.Init();
    }

    public virtual void ResetObject() {
        actor.Init();
    }

}
