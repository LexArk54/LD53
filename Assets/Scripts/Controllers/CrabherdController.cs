using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabherdController : ActorController {

    [SerializeField] bool debug;

    public Character player;
    public bool isAlerted;
    public bool isFollowing;
    public bool isGoingBack;

    [Header("Характеристики")]
    public float angleView = 120f;
    public float attackDistance = 1.5f;
    public float followDistance = 35f;
    [ReadOnly] public float radarRadus;
    [ReadOnly] public float radarRadusWarning;
    [ReadOnly] public float radarRadusDanger;

    [SerializeField] private LayerMask viewCollisionMask;
    [SerializeField] private Transform viewTransform;

    public override void Awake() {
        base.Awake();
        radarRadus = character.radar.GetRadius();
        radarRadusDanger = radarRadus / 5f;
        radarRadusWarning = radarRadus / 4f * 3f;
    }

    public override void OnActorReseted() {
        player = null;
        isAlerted = false;
        isFollowing = false;
        isGoingBack = false;
        UIManager.main.SetEYE(IndicatorColor.None);
        base.OnActorReseted();
    }

    private void OnEnable() {
        character.radar.OnEnter += Radar_OnEnter;
        character.radar.OnExit += Radar_OnExit;
    }

    private void OnDisable() {
        character.radar.OnEnter -= Radar_OnEnter;
        character.radar.OnExit -= Radar_OnExit;
    }

    private void Radar_OnEnter(Interactive iobject) {
        if (iobject.isPlayer) {
            player = (Character)iobject;
            UIManager.main.SetEYE(IndicatorColor.Success);
        }
    }

    private void Radar_OnExit(Interactive iobject) {
        if (iobject.isPlayer) {
            player = null;
            UIManager.main.SetEYE(IndicatorColor.None);
        }
    }


    public void TryAlert(Vector3 directionToPlayer, float distance, bool angleIsChecked) {
        if (player) {
            if (Physics.Raycast(character.model.headPoint.position, directionToPlayer, distance, viewCollisionMask)) {
                UIManager.main.SetEYE(IndicatorColor.Warning);
            } else if (!angleIsChecked) {
                directionToPlayer = directionToPlayer.SetY();
                character.movement.InputDirection(directionToPlayer);
                if (GetAngleView(directionToPlayer) < angleView) {
                    Alert();
                }
            } else {
                Alert();
            }
        }
    }
    public void Alert() {
        isAlerted = true;
        if (player) {
            UIManager.main.SetEYE(IndicatorColor.Danger);
            if (isGoingBack) {
                StopGoBack();
                StartFollow();
                return;
            }
        }
        character.model.animator.Play("Roar");
        StopAllCoroutines();
        StartCoroutine(_StartFollowIE());
    }

    private IEnumerator _StartFollowIE() {
        yield return new WaitForSeconds(.8f);
        StartFollow();
    }

    public void StartFollow() {
        isFollowing = true;
    }

    public void StopFollow() {
        isFollowing = false;
        StartGoBack();
    }

    public void StartGoBack() {
        isGoingBack = true;
        if (player) {
            UIManager.main.SetEYE(IndicatorColor.Warning);
        }
    }

    public void StopGoBack() {
        isGoingBack = false;
    }

    private float GetAngleView(Vector3 direction) {
        return Vector3.Angle((-viewTransform.right).SetY(), direction);
    }

    private void FixedUpdate() {
        if (debug) {
            var dir = (-viewTransform.right).SetY() * radarRadusWarning;
            Debug.DrawLine(character.model.headPoint.position, character.model.headPoint.position + Quaternion.Euler(0, -25, 0) * dir);
            Debug.DrawLine(character.model.headPoint.position, character.model.headPoint.position + Quaternion.Euler(0, 25, 0) * dir);
        }
        if (!isAlerted) {
            if (player && !player.isDead) {
                var directionToPlayer = player.model.collider.bounds.center - character.model.headPoint.position;
                var distance = directionToPlayer.magnitude;
                if (distance < radarRadusDanger) {
                    TryAlert(directionToPlayer, distance, false);
                    if (!isGoingBack) return;
                }
                if (distance < radarRadusWarning) {
                    if (!player.movement.isCrouching) {
                        TryAlert(directionToPlayer, distance, false);
                        if (!isGoingBack) return;
                    }
                    if (GetAngleView(directionToPlayer.SetY()) < angleView) {
                        TryAlert(directionToPlayer, distance, true);
                        if (!isGoingBack) return;
                    }
                    UIManager.main.SetEYE(IndicatorColor.Warning);
                } else {
                    UIManager.main.SetEYE(IndicatorColor.Success);
                }
                if (!isGoingBack) return;
            }
        }
        if (isGoingBack) {
            var distance = Vector3.Distance(transform.position, character.startPos);
            if (isAlerted && distance < radarRadusWarning) {
                isAlerted = false;
            }
            if (distance > .2f) {
                character.movement.MoveTo(character.startPos, false, true);
            } else {
                transform.position = character.startPos;
                character.movement.InputMove(Vector3.zero);
                character.movement.InputDirection(character.startRot * Vector3.forward);
                StopGoBack();
            }
        } else if (isFollowing) {
            if (player && !player.isDead) {
                if (Vector3.Distance(transform.position, character.startPos) > followDistance) {
                    StopFollow();
                } else if (Vector3.Distance(transform.position, player.transform.position) > attackDistance) {
                    character.movement.MoveTo(player.transform.position, false, true);
                } else {
                    var controller = player.GetComponent<PlayerController>();
                    controller.Kill(character);
                    StopFollow();
                }
                return;
            }
        }
    }

}
