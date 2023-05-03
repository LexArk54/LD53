using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeDeathTrigger : DeathTrigger {

    [SerializeField] private Rigidbody[] bridgeConstructs;
    private Vector3[] startPoss;
    private Quaternion[] startRots;
    public int deathCount = 2;

    [Header("Автоматические списки")]
    public int inRadarCount;

    private void Awake() {
        startPoss = new Vector3[bridgeConstructs.Length];
        startRots = new Quaternion[bridgeConstructs.Length];
        for (int i = 0; i < bridgeConstructs.Length; i++) {
            startPoss[i] = bridgeConstructs[i].position;
            startRots[i] = bridgeConstructs[i].rotation;
        }
    }

    public override void ResetObject() {
        base.ResetObject();
        for (int i = 0; i < bridgeConstructs.Length; i++) {
            bridgeConstructs[i].isKinematic = true;
            bridgeConstructs[i].velocity = Vector3.zero;
            bridgeConstructs[i].angularVelocity = Vector3.zero;
            bridgeConstructs[i].position = startPoss[i];
            bridgeConstructs[i].rotation = startRots[i];
        }
    }

    public void Crash() {
        for (int i = 0; i < bridgeConstructs.Length; i++) {
            bridgeConstructs[i].isKinematic = false;
        }
    }



    protected virtual void Enter(Interactive actor) {
        inRadarCount++;
        actor.OnTriggeringDisable += Exit;
        if (inRadarCount >= deathCount) {
            Crash();
        }
    }

    protected virtual void Exit(Interactive actor) {
        inRadarCount--;
        actor.OnTriggeringDisable -= Exit;
    }



    protected override void OnTriggerEnter(Collider other) {
        if (other.tag == Tag.Interactive) {
            var actor = other.GetComponentInParent<Interactive>();
            if (actor.data.type == InteractData.Type.Character) {
                Enter(actor);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == Tag.Interactive) {
            var actor = other.GetComponentInParent<Interactive>();
            if (actor.data.type == InteractData.Type.Character) {
                Exit(actor);
            }
        }
    }


}
