using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : ActorBase {

    public bool isPlayer => controller is PlayerController;
    public ActorController controller;

    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;

    public new ActorAudio audio { get; private set; }
    public ActorModel model { get; private set; }
    public ActorMovement movement { get; private set; }
    //public ActorInventory inventory { get; private set; }
    public ActorRadar radar { get; private set; }
    public ActorInteractivity interact { get; private set; }

    public bool isDead { get; private set; }
    public bool isUnderControl { get; set; }

    private bool _isInited = false;
    public void Init(ActorController controller) {
        if (!_isInited) {
            this.controller = controller;
            startPos = transform.position;
            startRot = transform.rotation;
            audio = GetComponentInChildren<ActorAudio>();
            model = GetComponentInChildren<ActorModel>();
            movement = GetComponent<ActorMovement>();
            //inventory = GetComponentInChildren<ActorInventory>();
            radar = GetComponentInChildren<ActorRadar>();
            interact = GetComponentInChildren<ActorInteractivity>();
            _isInited = true;
        } else {
            transform.position = startPos;
            transform.rotation = startRot;
        }
        audio?.Init(this);
        model?.Init(this);
        movement?.Init(this);
        //inventory?.Init(this);
        radar?.Init(this);
        interact?.Init(this);
        isUnderControl = true;
    }

    public void SetIsDead(bool value) {
        isDead = value;
        //if (idea) {
        //    var renders = idea.GetComponentsInChildren<Renderer>();
        //    foreach (var render in renders) {
        //        render.enabled = !value;
        //    }
        //}
    }

    public Action<Actor> OnTriggeringDisable;
    protected virtual void OnDisable() => TriggeringDisable();
    protected virtual void TriggeringDisable() => OnTriggeringDisable?.Invoke(this);

}
