using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : Item {

    public override void OnUse(Actor actor) {
        base.OnUse(actor);
        actor.interact.DropItem();
    }

}
