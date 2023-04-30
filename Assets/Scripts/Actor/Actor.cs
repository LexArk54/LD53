using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : ActorBase {

    public bool isPlayer = false;

    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;

    public new ActorAudio audio { get; private set; }
    public ActorModel model { get; private set; }
    public ActorMovement movement { get; private set; }
    //public ActorInventory inventory { get; private set; }
    public ActorRadar radar { get; private set; }
    public ActorInteractivity interact { get; private set; }

    public bool isActive { get; set; } = true;
    public bool isHidden { get; private set; }

    private void Awake() {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void Init() {
        transform.position = startPos;
        transform.rotation = startRot;
        audio = GetComponentInChildren<ActorAudio>();
        model = GetComponentInChildren<ActorModel>();
        movement = GetComponent<ActorMovement>();
        //inventory = GetComponentInChildren<ActorInventory>();
        radar = GetComponentInChildren<ActorRadar>();
        interact = GetComponentInChildren<ActorInteractivity>();
        audio?.Init(this);
        model?.Init(this);
        movement?.Init(this);
        //inventory?.Init(this);
        radar?.Init(this);
        interact?.Init(this);
    }

    public void SetHidden(bool value) {
        isHidden = value;
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
