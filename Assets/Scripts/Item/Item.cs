using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class ItemSlot {

    public ItemData data;
    public int count;

}

public class Item : ActorBase {

    [SerializeField] private bool isTakeable = true;

    [HideInInspector] public Rigidbody rigidBody;

    [Header("Только на сцене")]
    public bool isRespawner;
    [SerializeField] private float respawnDelay = -1f;
    protected Item respawner;

    public Actor hander;

    public virtual void Awake() {
        rigidBody = GetComponent<Rigidbody>();
        if (isRespawner) {
            respawner = this;
        }
    }

    public virtual bool CanUse(Actor actor) {
        return true;
    }
    public virtual void OnUse(Actor actor) {

    }
    public virtual void OnRadarIn(Actor actor) {

    }
    public virtual void OnRadarOut(Actor actor) {

    }
    public virtual bool CanTake(Actor actor) {
        return isTakeable;
    }
    public virtual Item OnTake(Actor actor) {
        if (isRespawner) {
            gameObject.SetActive(false);
            var item = Instantiate(data.prefab, transform.position, transform.rotation);
            item.isRespawner = false;
            item.respawner = respawner;
            return item.OnTake(actor);
        } else {
            hander = actor;
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;
            return this;
        }
    }
    public virtual void OnDrop(Actor actor) {
        hander = null;
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
    }

    private void Update() {
        if (hander) {
            transform.position = hander.model.armPoint.position;
            transform.rotation = hander.model.armPoint.rotation;
        }
    }

    public virtual void DestroyAndTryRespawn(bool respawnDelay = true) {
        if (hander) {
            hander.interact.DropItem();
        }
        if (isTakeable) {
            if (respawnDelay) {
                GameManager.main.StartCoroutine(_RespawnDelay());
            } else {
                respawner.gameObject.SetActive(true);
            }
            gameObject.SetActive(false);
            if (!isRespawner && respawner) {
                Destroy(gameObject);
            }
        } else {
            Destroy(gameObject);
        }
    }

    protected virtual IEnumerator _RespawnDelay() {
        yield return new WaitForSeconds(respawner.respawnDelay);
        respawner.gameObject.SetActive(true);
    }

    public Action<Item> OnTriggeringDisable;
    protected virtual void OnDisable() => TriggeringDisable();
    protected virtual void TriggeringDisable() => OnTriggeringDisable?.Invoke(this);

}
