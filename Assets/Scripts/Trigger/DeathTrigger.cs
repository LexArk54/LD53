using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.tag == Tag.Actor) {
            var actor = other.GetComponentInParent<Actor>();
            if (actor.isPlayer) {
                actor.GetComponent<PlayerController>().FallDeath();
            } else {
                actor.GetComponent<ActorController>().ResetObject();
            }
        } else if (other.tag == Tag.Item) {
            other.GetComponent<Item>().DestroyAndTryRespawn(true);
        }
    }

}
