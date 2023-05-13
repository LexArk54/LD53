using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : Item {

    [SerializeField] private FishRadar radar;

    [SerializeField] private List<Fish> additiveFishes = new List<Fish>();

    public override void ResetObject() {
        if (hander) {
            UIManager.main.SetFishCount(0);
        }
        base.ResetObject();
        radar.gameObject.SetActive(false);
        additiveFishes.Clear();
    }

    public void OnEated() {
        if (hander && additiveFishes.Count > 0) {
            var fish = additiveFishes[0];
            OnDrop(hander);
            fish.OnEated();
        } else {
            ResetObject();
        }
    }

    public override void OnUse(Character character) {
        base.OnUse(character);
        character.interact.PlaceItem();
    }

    protected override bool CanTake(Character actor) {
        return base.CanTake(actor) || actor.interact.itemInHand.data.name == ActorNames.Fish;
    }

    public override Item OnTake(Character actor) {
        if (actor.interact.itemInHand && actor.interact.itemInHand.data.name == ActorNames.Fish) {
            var fishInHand = (Fish)actor.interact.itemInHand;
            fishInHand.additiveFishes.Add(this);
            gameObject.SetActive(false);
            fishInHand.AddFishesInCounter(actor);
            UIManager.main.SetFishCount(fishInHand.additiveFishes.Count + 1);
            return null;
        } else {
            base.OnTake(actor);
            radar.gameObject.SetActive(true);
            AddFishesInCounter(actor);
            UIManager.main.SetFishCount(additiveFishes.Count + 1);
            return this;
        }
    }

    private void AddFishesInCounter(Character actor) {
        var fish = (Fish)actor.radar.GetNearest(ActorNames.Fish, CharacterRadar.HandState.NotInHand);
        while (fish != null) {
            additiveFishes.Add(fish);
            fish.gameObject.SetActive(false);
            fish = (Fish)actor.radar.GetNearest(ActorNames.Fish, CharacterRadar.HandState.NotInHand);
        }
    }

    public override Item OnDrop(Character actor) {
        if (additiveFishes.Count > 0) {
            var fish = additiveFishes[0];
            additiveFishes.RemoveAt(0);
            UIManager.main.SetFishCount(additiveFishes.Count + 1);
            fish.additiveFishes.Clear();
            fish.gameObject.SetActive(true);
            fish.OnDrop(actor);
            fish.transform.position = transform.position;
            fish.transform.rotation = transform.rotation;
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
