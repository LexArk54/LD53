using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour {

    public virtual void ResetObject() {

    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.tag == Tag.Interactive) {
            var actor = other.GetComponentInParent<Interactive>();
            if (actor.data.type == InteractData.Type.Character) {
                if (actor.isPlayer) {
                    actor.GetComponent<PlayerController>().FallDeath();
                } else {
                    actor.GetComponent<CharacterController>().ResetObject();
                }
            } else {
                actor.ResetObject();
            }
        }
    }

}
