using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Interactive {

    [HideInInspector] public ActorController controller;

    public new CharacterAudio audio { get; private set; }
    public CharacterModel model { get; private set; }
    public CharacterMovement movement { get; private set; }
    public CharacterRadar radar { get; private set; }
    public PlayerInteractivity interact { get; private set; }

    public bool isDead { get; private set; }
    public bool isUnderControl { get; set; }
    public void Init(ActorController controller) {
        this.controller = controller;
        audio = GetComponentInChildren<CharacterAudio>();
        model = GetComponentInChildren<CharacterModel>();
        movement = GetComponent<CharacterMovement>();
        radar = GetComponentInChildren<CharacterRadar>();
        interact = GetComponentInChildren<PlayerInteractivity>();
        audio?.Init(this);
        model?.Init(this);
        movement?.Init(this);
        radar?.Init(this);
        interact?.Init(this);
        isUnderControl = true;
    }

    public override void ResetObject() {
        base.ResetObject();
        audio?.Init(this);
        model?.Init(this);
        movement?.Init(this);
        radar?.Init(this);
        interact?.Init(this);
        isUnderControl = true;
        var items = FindObjectsByType<Item>(FindObjectsSortMode.None);
        foreach (var item in items) {
            item.ResetObject();
        }
        controller.OnActorReseted();
    }

    public void SetIsDead(bool value) {
        isDead = value;
        interact.StopAllCoroutines();
    }

    public override bool CanInteract(Character character) {
        return controller.CanInteract(character);
    }

    public override void Interact(Character character) {
        base.Interact(character);
        controller.Interact(character);
    }

}
