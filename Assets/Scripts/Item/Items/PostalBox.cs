using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostalBox : Item {

    public GameObject container;
    public Transform cameraFlowTarget;

    public override bool CanUse(Character actor) {
        return base.CanUse(actor) && !container.activeSelf;
    }

    public override void OnUse(Character actor) {
        base.OnUse(actor);
        var player = actor.GetComponent<PlayerController>();
        actor.transform.forward = (transform.position - actor.transform.position).SetY();
        player.enabled = false;
        actor.interact.Kick((int state) => {
            if (state == 0) return;
            player.Pause();
            player.enabled = false;
            UIManager.main.CameraFlow(cameraFlowTarget, 0f, 1f, 2f, (int step) => {
                if (step == 1) {
                    container.SetActive(true);
                } else if (step == 3) {
                    player.enabled = true;
                    player.Play();
                }
            });
        });
    }

}
