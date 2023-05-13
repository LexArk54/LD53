using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Item : Interactive {

    [SerializeField] protected bool isTakeable = true;


    [HideInInspector] public Rigidbody rigidBody;

    private bool isKinematic;

    public override void ResetObject() {
        if (isTakeable) {
            if (!gameObject.activeSelf) {
                gameObject.SetActive(true);
            }
            if (hander) {
                if (gameObject.activeSelf) {
                    hander.interact.DropItem(true);
                } else {
                    hander = null;
                }
            }
            rigidBody.isKinematic = isKinematic;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
        base.ResetObject();
    }

    public override void Awake() {
        rigidBody = GetComponent<Rigidbody>();
        base.Awake();
        isKinematic = rigidBody.isKinematic;
    }

    public override bool CanInteract(Character character) {
        if (isTakeable && hander == null) {
            return CanTake(character);
        } else {
            return CanUse(character);
        }
    }

    public override void Interact(Character character) {
        base.Interact(character);
        if (isTakeable && hander == null) {
            if (CanTake(character)) {
                character.interact.PickupItem(this);
            }
        } else {
            if (CanUse(character)) {
                OnUse(character);
            }
        }
    }

    protected virtual bool CanUse(Character actor) {
        return true;
    }
    protected virtual bool CanTake(Character actor) {
        return !actor.interact.itemInHand;
    }
    public virtual Item OnTake(Character actor) {
        hander = actor;
        rigidBody.isKinematic = true;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        return this;
    }
    public virtual Item OnDrop(Character actor) {
        hander = null;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.isKinematic = false;
        return null;
    }

    private void Update() {
        if (hander) {
            transform.position = hander.model.armPoint.position;
            transform.rotation = hander.model.armPoint.rotation;
        }
    }

}
