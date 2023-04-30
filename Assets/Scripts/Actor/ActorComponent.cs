using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorComponent : MonoBehaviour {

    [HideInInspector] public Actor actor;

    public virtual void Init(Actor actor) {
        this.actor = actor;
    }

}
