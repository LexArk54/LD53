using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigController : CharacterController {

    public Character player;
    public bool isAlerted;
    public bool isFollowing;
    public bool isGoingBack;

    [Header("Характеристики")]
    public float angleView = 140f;
    public float attackDistance = 1.5f;
    public float followDistance = 20f;
    [ReadOnly] public float radarRadus;
    [ReadOnly] public float radarRadusWarning;
    [ReadOnly] public float radarRadusDanger;

    [SerializeField] private LayerMask viewCollisionMask;

    public override void Awake() {
        base.Awake();
        radarRadus = character.radar.GetRadius();
        radarRadusDanger = radarRadus / 4f;
        radarRadusWarning = radarRadusDanger * 3f;
    }

    public override void ResetObject() {
        player = null;
        isAlerted = false;
        isFollowing = false;
        isGoingBack = false;
        base.ResetObject();
    }

    private void OnEnable() {
        character.radar.OnEnter += Radar_OnEnter;
        character.radar.OnExit += Radar_OnExit;
    }

    private void OnDisable() {
        character.radar.OnEnter -= Radar_OnEnter;
        character.radar.OnExit -= Radar_OnExit;
    }

    private void Radar_OnEnter(Interactive actor) {
        if (actor.isPlayer) {
            player = (Character)actor;
            UIManager.main.SetEYE(IndicatorColor.Success);
        }
    }

    private void Radar_OnExit(Interactive actor) {
        if (actor.isPlayer) {
            player = null;
            UIManager.main.SetEYE(IndicatorColor.None);
        }
    }


    public bool TryAlert(Vector3 directionToPlayer, float distance) {
        if (player) {
            if (Physics.Raycast(character.model.headPoint.position, directionToPlayer, distance, viewCollisionMask)) {
                UIManager.main.SetEYE(IndicatorColor.Warning);
            } else {
                Alert();
                return true;
            }
        }
        return false;
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

    private void FixedUpdate() {
        if (!isAlerted) {
            if (player && !player.isDead) {
                var directionToPlayer = player.model.collider.bounds.center - character.model.headPoint.position;
                var distance = directionToPlayer.magnitude;
                if (distance < radarRadusDanger) {
                    character.movement.InputDirection(directionToPlayer.SetY());
                }
                if (distance < radarRadusWarning) {
                    if (!player.movement.isCrouching) {
                        TryAlert(directionToPlayer, distance);
                        if (!isGoingBack) return;
                    }
                    if (Vector3.Angle(transform.forward, directionToPlayer.SetY()) < angleView) {
                        TryAlert(directionToPlayer, distance);
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
                character.movement.MoveTo(character.startPos, true);
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
                    character.movement.MoveTo(player.transform.position, false);
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
