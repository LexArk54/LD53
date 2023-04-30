using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorInteractivity : ActorComponent {

    public CrabController crabInHands;
    public Item itemInHand;

    public override void Init(Actor actor) {
        base.Init(actor);
        if (itemInHand) {
            itemInHand.DestroyAndTryRespawn(false);
        }
        if (crabInHands) {
            crabInHands.ResetObject();
        }
        itemInHand = null;
        crabInHands = null;
    }

    public void TakeItem(Item item) {
        itemInHand = item.OnTake(actor);
    }

    public void DropItem() {
        if (itemInHand == null) return;
        itemInHand.OnDrop(actor);
        itemInHand = null;
    }

    public void TakeCrab(CrabController crab) {
        crab.SetInHands(actor);
        crabInHands = crab;
        actor.model.animator.SetBool("crabInHands", true);
    }

    public void DropCrab() {
        if (crabInHands == null) return;
        crabInHands.SetInHands(null);
        crabInHands = null;
        actor.model.animator.SetBool("crabInHands", false);
    }

    public void InputUse() {
        if (actor.movement.isSprinting) return;
        if (crabInHands) {
            InputDrop();
            return;
        }
        if (itemInHand) {
            if (itemInHand.CanUse(actor)) {
                itemInHand.OnUse(actor);
            }
            return;
        }
        var actorBase = actor.radar.GetNearest();
        if (actorBase) {
            if (actorBase is Item) {
                var item = (Item)actorBase;
                if (item.CanTake(actor)) {
                    TakeItem(item);
                } else if (item.CanUse(actor)) {
                    item.OnUse(actor);
                }
            } else {
                var crab = actorBase.GetComponent<CrabController>();
                if (crab) {
                    TakeCrab(crab);
                }
            }
        }
    }

    public void InputDrop() {
        DropItem();
        DropCrab();
    }

}
