using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostalBox : Item {

    public GameObject container;
    public Transform cameraFlowTarget;

    public override bool CanUse(Actor actor) {
        return base.CanUse(actor) && !container.activeSelf;
    }

    public override void OnUse(Actor actor) {
        base.OnUse(actor);
        var player = actor.GetComponent<PlayerController>();
        player.Pause();
        player.enabled = false;
        UIManager.main.CameraFlow(cameraFlowTarget, 1f, 1f, 2f, (int step) => {
            if (step == 1) {
                container.SetActive(true);
            } else if (step == 3) {
                player.enabled = true;
                player.Play();
            }
        });
    }

}
