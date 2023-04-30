using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigController : ActorComponent {

    public Actor player;
    public bool isFollowing;

    [Header("Характеристики")]
    public float angleView = 140f;
    public float attackDistance = 1.5f;
    public float followDistance = 20f;
    [ReadOnly] public float radarRadus;
    [ReadOnly] public float radarRadusWarning;
    [ReadOnly] public float radarRadusDanger;

    NavMeshPath path;

    public void Awake() {
        Init(GetComponent<Actor>());
        actor.Init();
        radarRadus = actor.radar.GetRadius();
        radarRadusDanger = radarRadus / 3f;
        radarRadusWarning = radarRadusDanger * 2f;
    }

    public void ResetObject() {
        player = null;
        isFollowing = false;
        actor.Init();
    }

    private void OnEnable() {
        actor.radar.OnEnter += Radar_OnEnter;
        actor.radar.OnExit += Radar_OnExit;
    }

    private void OnDisable() {
        actor.radar.OnEnter -= Radar_OnEnter;
        actor.radar.OnExit -= Radar_OnExit;
    }

    private void Radar_OnEnter(Actor actor) {
        if (actor.isPlayer) {
            player = actor;
        }
    }

    private void Radar_OnExit(Actor actor) {
        if (actor.isPlayer) {
            player = null;
            isFollowing = false;
        }
    }

    private void FixedUpdate() {
        if (player) {
            if (!isFollowing) {
                var directionToPlayer = player.transform.position - transform.position;
                var distance = directionToPlayer.magnitude;
                if (distance < radarRadusDanger) {
                    isFollowing = true;
                    return;
                }
                if (distance < radarRadusWarning) {
                    if (!player.movement.isCrouching) {
                        isFollowing = true;
                        return;
                    }
                    if (directionToPlayer.y < 4f) {
                        directionToPlayer.y = 0;
                        if (Vector3.Angle(transform.forward, directionToPlayer) < angleView) {
                            isFollowing = true;
                            return;
                        }
                    }
                }
            } else {
                if (Vector3.Distance(transform.position, actor.startPos) > followDistance) {
                    Radar_OnExit(player);
                } else if (Vector3.Distance(transform.position, player.transform.position) > attackDistance) {
                    MoveTo(player.transform.position, false);
                } else {
                    Debug.Log("kill");
                    player.GetComponent<PlayerController>().ResetObject();
                    foreach (var a in actor.radar.actors) {
                        if (!a.isPlayer && (player.interact.crabInHands == null || player.interact.crabInHands.actor != a)) {
                            a.GetComponent<CrabController>().ResetObject();
                        }
                    }
                    ResetObject();
                }
                return;
            }
        }
        if (Vector3.Distance(transform.position, actor.startPos) > .3f) {
            MoveTo(actor.startPos, true);
        } else if (transform.position != actor.startPos) {
            transform.position = actor.startPos;
            actor.movement.InputMove(Vector3.zero);
            actor.movement.InputDirection(actor.startRot * Vector3.forward);
        }
    }

    private void MoveTo(Vector3 pos, bool smooth) {
        path = new NavMeshPath();
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas)) {
            NavMesh.CalculatePath(hit.position, pos, -1, path);
        }
        Vector3 moveDirection;
        Vector3 destinationDirection;
        if (path.corners.Length == 0) {
            moveDirection = pos - transform.position;
            destinationDirection = moveDirection;
        } else {
            moveDirection = path.corners[1] - transform.position;
            destinationDirection = pos - transform.position;
            destinationDirection.y = 0;
        }
        moveDirection.y = 0;
        actor.movement.InputDirection(moveDirection.normalized);
        if (smooth) {
            moveDirection = transform.forward * (destinationDirection.magnitude / actor.movement.runSpeed);
            actor.movement.InputMove(Vector3.ClampMagnitude(moveDirection, 1f));
        } else {
            actor.movement.InputMove(transform.forward);
        }
        actor.movement.InputSprint();
    }

}
