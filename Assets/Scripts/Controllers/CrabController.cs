using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabController : ActorController {

    public Item target;
    public float targetDistance;

    public Actor hander;

    [Header("����������")]
    public float eating;
    public bool isEating;
    public bool isFollowing;

    [Header("��������������")]
    public float targetDelay = 2f;
    public float eatingTime = 3f;
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
            actor.movement.InputMove(Vector3.zero);
        }
    }
    IEnumerator _TargetDelay() {
        yield return new WaitForSeconds(targetDelay);
        isFollowing = target != null;
    }

    public void StartEating() {
        actor.movement.InputMove(Vector3.zero);
        eating = eatingTime;
        isEating = true;
        actor.model.animator.SetBool("isEating", true);
    }

    public void StopEating() {
        isEating = false;
        actor.model.animator.SetBool("isEating", false);
        if (target) {
            if (eating < 0) {
                target.DestroyAndTryRespawn();
                SetTarget(actor.radar.GetNearestItem(ItemNames.Fish));
            } else if (!target.hander) {
                SetTarget(null);
            }
        }
        eating = 0;
    }

    public void SetInHands(Actor hander) {
        this.hander = hander;
        if (hander) {
            actor.movement.InputMove(Vector3.zero);
            actor.movement.rigidBody.detectCollisions = false;
            actor.movement.rigidBody.useGravity = false;
            actor.movement.rigidBody.isKinematic = true;
            actor.movement.enabled = false;
            actor.movement.Stop();
            SetTarget(null);
            Update();
        } else {
            actor.movement.rigidBody.detectCollisions = true;
            actor.movement.rigidBody.useGravity = true;
            actor.movement.rigidBody.isKinematic = false;
            actor.movement.enabled = true;
            actor.movement.InputDirection(transform.forward);
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
            SetTarget(actor.radar.GetNearestItem(ItemNames.Fish));
        }
        if (target) {
            if (isFollowing) {
                if (targetDistance > eatDistance) {
                    MoveTo(target.transform.position, false);
                } else {
                    if (!isEating) {
                        StartEating();
                    }
                }
            } else {
                actor.movement.InputDirection((target.transform.position - transform.position).SetY());
            }
            return;
        }
    }

    private void MoveTo(Vector3 pos, bool smooth) {
        path = new NavMeshPath();
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, Layer.NavMesh.PathFind)) {
            NavMesh.CalculatePath(hit.position, pos, Layer.NavMesh.PathFind, path);
        }
        Vector3 moveDirection;
        if (path.corners.Length == 0) {
            moveDirection = (pos - transform.position).SetY();
        } else {
            moveDirection = (path.corners[1] - transform.position).SetY();
        }
        actor.movement.InputDirection(moveDirection.normalized);
        if (smooth) {
            moveDirection = transform.forward * (targetDistance / actor.movement.runSpeed);
            actor.movement.InputMove(Vector3.ClampMagnitude(moveDirection, 1f));
        } else {
            actor.movement.InputMove(transform.forward);
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
