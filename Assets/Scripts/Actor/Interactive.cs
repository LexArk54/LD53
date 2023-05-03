using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour {

    public bool isPlayer => data.name == ActorNames.Player;
    public bool isCrab => data.name == ActorNames.Player;
    public bool isCraherd => data.name == ActorNames.Crabherd;
    public InteractData data;

    public Character hander;

    [TextArea(3,3)] [SerializeField] protected string hintInHand;
    [TextArea(3, 3)] [SerializeField] protected string hintTake;

    public virtual void ResetObject() { }

    public virtual string GetHit() {
        return data.name + "\n" + (hander ? hintInHand : hintTake);
    }

    public virtual bool CanUse(Character character) {
        return hander == null || hander == character.interact.itemInHand;
    }

    public virtual void OnUse(Character character) {
    }

    public virtual void OnActorRadarEnter(Character character) {

    }
    public virtual void OnActorRadarExit(Character character) {

    }

    public Action<Interactive> OnTriggeringDisable;
    protected virtual void OnDisable() => TriggeringDisable();
    protected virtual void TriggeringDisable() => OnTriggeringDisable?.Invoke(this);

}
