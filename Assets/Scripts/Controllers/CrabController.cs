using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabController : CharacterController {

    public Item target;
    public float targetDistance;

    public Character hander;

    [Header("����������")]
    public float eating;
    public bool isEating;
    public bool isFollowing;

    [Header("��������������")]
    public float targetDelay = 2f;
    public float eatingTime = 2f;
    public float eatDistance = 1f;

    NavMeshPath path;

    public override void ResetObject() {
        eating = 0;
        isFollowing = false;
        hander = null;
        target = null;
        StopEating();
        SetInHands(null);
        base.ResetObject();
    }

    public void SetTarget(Item target) {
        if (this.target == target) return;
        this.target = target;
        if (target) {
            if (!isFollowing) {
                StopAllCoroutines();
                StartCoroutine(_TargetDelay());
            }
        } else {
            if (isEating) StopEating();
            isFollowing = false;
            character.movement.InputMove(Vector3.zero);
        }
    }
    IEnumerator _TargetDelay() {
        yield return new WaitForSeconds(targetDelay);
        isFollowing = target != null;
    }

    public void StartEating() {
        character.movement.InputMove(Vector3.zero);
        eating = eatingTime;
        isEating = true;
        character.model.animator.SetBool("isEating", true);
    }

    public void StopEating() {
        isEating = false;
        character.model.animator.SetBool("isEating", false);
        if (target) {
            if (eating < 0) {
                target.ResetItem();
                SetTarget((Fish)character.radar.GetNearest(ActorNames.Fish));
            } else if (!target.hander) {
                SetTarget(null);
            }
        }
        eating = 0;
    }

    public void SetInHands(Character hander) {
        if (hander) {
            Physics.IgnoreLayerCollision(gameObject.layer, hander.gameObject.layer, true);
            character.movement.InputMove(Vector3.zero);
            character.movement.rigidBody.isKinematic = true;
            character.movement.enabled = false;
            character.movement.Stop();
            SetTarget(null);
            this.hander = hander;
            Update();
        } else {
            if (this.hander) { 
                Physics.IgnoreLayerCollision(gameObject.layer, this.hander.gameObject.layer, false); 
            }
            character.movement.rigidBody.isKinematic = false;
            character.movement.enabled = true;
            character.movement.InputDirection(transform.forward);
            this.hander = null;
        }
    }

    private void FixedUpdate() {
        if (hander) return;
        if (target) {
            targetDistance = (target.transform.position - transform.position).SetY().magnitude;
        }
        if (isEating) {
            if (!target) {
                StopEating();
                return;
            }
            eating -= Time.fixedDeltaTime;
            if (eating <= 0 || targetDistance > eatDistance) {
                StopEating();
            }
            return;
        } else {
            SetTarget((Fish)character.radar.GetNearest(ActorNames.Fish));
        }
        if (target) {
            if (isFollowing) {
                if (targetDistance > eatDistance) {
                    character.movement.MoveTo(target.transform.position, false);
                } else {
                    if (!isEating) {
                        StartEating();
                    }
                }
            } else {
                character.movement.InputDirection((target.transform.position - transform.position).SetY());
            }
            return;
        }
    }

    public Vector3 GetHandsOffset() { return transform.right * -.4f + Vector3.up * -.25f + transform.forward * -.2f; }

    public void Update() {
        if (hander) {
            transform.position = hander.model.armPoint.position + GetHandsOffset();
            transform.rotation = hander.transform.rotation;
            return;
        }
    }

}
