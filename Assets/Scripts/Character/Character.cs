using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Interactive {

    [HideInInspector] public CharacterController controller;

    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;

    public new CharacterAudio audio { get; private set; }
    public CharacterModel model { get; private set; }
    public CharacterMovement movement { get; private set; }
    //public ActorInventory inventory { get; private set; }
    public CharacterRadar radar { get; private set; }
    public PlayerInteractivity interact { get; private set; }

    public bool isDead { get; private set; }
    public bool isUnderControl { get; set; }

    private bool _isInited = false;
    public void Init(CharacterController controller) {
        if (!_isInited) {
            this.controller = controller;
            startPos = transform.position;
            startRot = transform.rotation;
            audio = GetComponentInChildren<CharacterAudio>();
            model = GetComponentInChildren<CharacterModel>();
            movement = GetComponent<CharacterMovement>();
            radar = GetComponentInChildren<CharacterRadar>();
            interact = GetComponentInChildren<PlayerInteractivity>();
            _isInited = true;
        } else {
            transform.position = startPos;
            transform.rotation = startRot;
        }
        audio?.Init(this);
        model?.Init(this);
        movement?.Init(this);
        radar?.Init(this);
        interact?.Init(this);
        isUnderControl = true;
    }

    public void SetIsDead(bool value) {
        isDead = value;
    }

}
