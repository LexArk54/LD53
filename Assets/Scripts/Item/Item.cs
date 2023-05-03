using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class ItemSlot {

    public InteractData data;
    public int count;

}

public class Item : Interactive {

    [SerializeField] protected bool isTakeable = true;


    [HideInInspector] public Rigidbody rigidBody;

    private Vector3 startPos;
    private Quaternion startRot;
    private bool isKinematic;

    public virtual void Awake() {
        rigidBody = GetComponent<Rigidbody>();
        startPos = transform.position;
        startRot = transform.rotation;
        isKinematic = rigidBody.isKinematic;
    }

    public virtual bool CanTake(Character actor) {
        return isTakeable;
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

    public virtual void ResetItem() {
        if (isTakeable) {
            if (!gameObject.activeSelf) {
                gameObject.SetActive(true);
            }
            if (hander) {
                if (gameObject.activeSelf) {
                    hander.interact.DropAllItems();
                } else {
                    hander = null;
                    transform.SetParent(null);
                }
            }
            transform.position = startPos;
            transform.rotation = startRot;
            rigidBody.isKinematic = isKinematic;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

}
