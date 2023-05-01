using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigController : ActorController {

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

    public override void Awake() {
        base.Awake();
        radarRadus = actor.radar.GetRadius();
        radarRadusDanger = radarRadus / 4f;
        radarRadusWarning = radarRadusDanger * 3f;
    }

    public override void ResetObject() {
        player = null;
        isFollowing = false;
        base.ResetObject();
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
        if (player && !player.isDead) {
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
                    List<CrabController> crabs = new List<CrabController>();
                    foreach (var a in actor.radar.actors) {
                        if (!a.isPlayer) {
                            var crab = a.GetComponent<CrabController>();
                            if (crab) {
                                crabs.Add(crab);
                            }
                        }
                    }
                    var controller = player.GetComponent<PlayerController>();
                    controller.Kill(actor, () => {
                        foreach (var crab in crabs) {
                            crab.ResetObject();
                        }
                    });
                }
                return;
            }
        }
        if (Vector3.Distance(transform.position, actor.startPos) > .2f) {
            MoveTo(actor.startPos, true);
        } else if (transform.position != actor.startPos) {
            transform.position = actor.startPos;
            actor.movement.InputMove(Vector3.zero);
            actor.movement.InputDirection(actor.startRot * Vector3.forward);
        }
    }

    private void MoveTo(Vector3 pos, bool smooth) {
        path = new NavMeshPath();
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, Layer.NavMesh.PathFind)) {
            NavMesh.CalculatePath(hit.position, pos, Layer.NavMesh.PathFind, path);
        }
        Vector3 moveDirection;
        Vector3 destinationDirection;
        if (path.corners.Length == 0) {
            moveDirection = (pos - transform.position).SetY();
            destinationDirection = moveDirection;
        } else {
            moveDirection = (path.corners[1] - transform.position).SetY();
            destinationDirection = (pos - transform.position).SetY();
        }
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
