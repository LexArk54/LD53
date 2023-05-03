using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : Item {

    [SerializeField] private FishRadar radar;

    private List<Fish> additiveFishes = new List<Fish>();

    public override void ResetItem() {
        if (hander) {
            UIManager.main.SetFishCount(0);
        }
        base.ResetItem();
        additiveFishes.Clear();
        radar.gameObject.SetActive(false);
    }

    public override void OnUse(Character actor) {
        base.OnUse(actor);
        var fish = (Fish)actor.radar.GetNearest(ActorNames.Fish, CharacterRadar.HandState.NotInHand);
        if (fish) {
            actor.interact.PickupItem(fish);
            return;
        }
    }

    public override Item OnTake(Character actor) {
        if (actor.interact.itemInHand && actor.interact.itemInHand.data.name == ActorNames.Fish) {
            var fishInHand = (Fish)actor.interact.itemInHand;
            fishInHand.additiveFishes.Add(this);
            UIManager.main.SetFishCount(fishInHand.additiveFishes.Count + 1);
            hander = actor;
            transform.SetParent(actor.model.armPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            gameObject.SetActive(false);
            return null;
        } else {
            UIManager.main.SetFishCount(1);
            var fishInHand = (Fish)base.OnTake(actor);
            fishInHand.radar.gameObject.SetActive(true);
            fishInHand.additiveFishes.Clear();
            return fishInHand;
        }
    }

    public override Item OnDrop(Character actor) {
        if (additiveFishes.Count > 0) {
            var fish = additiveFishes[0];
            additiveFishes.RemoveAt(0);
            UIManager.main.SetFishCount(additiveFishes.Count + 1);
            fish.additiveFishes.Clear();
            fish.transform.SetParent(null);
            fish.gameObject.SetActive(true);
            fish.hander = null;
            fish.OnDrop(actor);
            return this;
        } else {
            if (hander) {
                UIManager.main.SetFishCount(0);
            }
            radar.gameObject.SetActive(false);
            return base.OnDrop(actor);
        }
    }

    public void OnFishRadarEnter(int count) {
        if (count > 0) {
            UIManager.main.SetFishColor(IndicatorColor.Success);
        }
    }

    public void OnFishRadarExit(int count) {
        if (count > 0) return;
        UIManager.main.SetFishColor(IndicatorColor.White);
    }

}
