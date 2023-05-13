using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour {

    public bool isPlayer => data.name == ActorNames.Player;
    public bool isCrab => data.name == ActorNames.Crab;
    public bool isCrabherd => data.name == ActorNames.Crabherd;

    public InteractData data;


    public Character hander;



    public Vector3 startPos { get; protected set; }
    public Quaternion startRot { get; protected set; }


    public virtual void Awake() {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public virtual void ResetObject() {
        transform.position = startPos;
        transform.rotation = startRot;
    }

    public virtual string GetHint() {
        var hint = (hander ? data.hintUseInhand : data.hintUse);
        if (hint != string.Empty) {
            return data.name + "\n" + hint + " [E]";
        }
        return string.Empty;
    }

    public virtual bool CanInteract(Character character) {
        return hander == null || this == character.interact.itemInHand;
    }
    public virtual void Interact(Character character) { }
    public virtual void OnUse(Character character) { }

    public virtual void OnActorRadarEnter(Character character) { }
    public virtual void OnActorRadarExit(Character character) { }


    public Action<Interactive> OnTriggeringDisable;
    protected virtual void OnDisable() => TriggeringDisable();
    protected virtual void TriggeringDisable() => OnTriggeringDisable?.Invoke(this);

}
