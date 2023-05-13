using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour {

    public virtual void ResetObject() {

    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.tag == Tag.Interactive) {
            var iobject = other.GetComponentInParent<Interactive>();
            if (iobject.data.type == InteractData.Type.Character) {
                if (iobject.isPlayer) {
                    iobject.GetComponent<PlayerController>().FallDeath();
                } else {
                    iobject.ResetObject();
                }
            } else {
                iobject.ResetObject();
            }
        }
    }

}
