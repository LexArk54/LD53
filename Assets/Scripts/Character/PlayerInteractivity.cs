using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractivity : CharacterComponent {

    public Interactive itemInHand;

    private Interactive hoveredObject;

    public override void Init(Character character) {
        base.Init(character);
        if (itemInHand) {
            itemInHand.ResetObject();
        }
        character.model.animator.SetBool("crabInHands", false);
        character.model.animator.ResetTrigger("PickUp");
        character.model.animator.ResetTrigger("PickDown");
        itemInHand = null;
    }

    public Interactive GetCrabInHands() {
        if (itemInHand && itemInHand.data.name == ActorNames.Crab) {
            return itemInHand;
        }
        return null;
    }

    public void TakeItem(Item item) {
        var itemIn = item.OnTake(character);
        if (itemIn) {
            itemInHand = itemIn;
            if (hoveredObject == itemInHand) {
                hoveredObject = null;
            }
        }
    }

    public void DropItem() {
        if (itemInHand == null) return;
        itemInHand = (itemInHand as Item).OnDrop(character);
        hoveredObject = itemInHand;
    }
    public void DropAllItems() {
        while (itemInHand) {
            DropItem();
        }
    }

    public void PickupCrab(CrabController crab) {
        crab.SetInHands(character);
        itemInHand = crab.character;
        character.model.animator.SetBool("crabInHands", true);
    }

    public void DropCrab() {
        if (itemInHand == null || itemInHand is Item) return;
        itemInHand.GetComponent<CrabController>().SetInHands(null);
        itemInHand = null;
        character.model.animator.SetBool("crabInHands", false);
    }

    private void UpdateHovered() {
        Interactive hovered;
        if (character.movement.isSprinting) {
            hovered = null;
        } else {
            hovered = character.radar.GetNearest(InteractData.Type.All, CharacterRadar.HandState.NotInHand);
            if (!hovered || !hovered.CanUse(character)) {
                hovered = itemInHand;
            }
        }
        if (hoveredObject != hovered) {
            hoveredObject = hovered;
            if (hovered) {
                UIManager.main.SetItemHint(hovered.GetHit());
            } else {
                UIManager.main.SetItemHint("");
            }
        }
    }

    private void FixedUpdate() {
        UpdateHovered();
    }

    public void InputUse() {
        if (character.movement.isSprinting) return;
        if (itemInHand) {
            if (itemInHand.CanUse(character)) {
                itemInHand.OnUse(character);
            }
            return;
        }
        if (hoveredObject) {
            if (hoveredObject is Item) {
                var item = (Item)hoveredObject;
                if (item.CanTake(character)) {
                    PickupItem(item);
                } else if (item.CanUse(character)) {
                    item.OnUse(character);
                }
            } else {
                var crab = hoveredObject.GetComponent<CrabController>();
                if (crab) {
                    PickupCrab(crab);
                }
            }
        }
    }
    public void InputDrop() {
        if (character.movement.isSprinting) return;
        PlaceItem();
        DropCrab();
    }

    public void PickupItem(Item item) {
        StartCoroutine(_PickupItem(item));
    }
    IEnumerator _PickupItem(Item item) {
        character.movement.InputDirection((item.transform.position - transform.position).SetY());
        character.controller.Pause();
        character.model.animator.SetTrigger("PickUp");
        yield return new WaitForSeconds(.5f);
        TakeItem(item);
        yield return new WaitForSeconds(.6f);
        character.controller.Play();
    }

    public void PlaceItem() {
        if (!itemInHand) return;
        StartCoroutine(_PlaceItem());
    }
    IEnumerator _PlaceItem() {
        character.controller.Pause();
        character.model.animator.SetTrigger("PickDown");
        yield return new WaitForSeconds(.5f);
        DropItem();
        yield return new WaitForSeconds(.6f);
        character.controller.Play();
    }

    public void Kick(Action<int> callback = null) {
        StartCoroutine(_KickEI(callback));
    }
    IEnumerator _KickEI(Action<int> callback) {
        character.controller.Pause();
        character.model.animator.Play("Kick");
        yield return new WaitForSeconds(.5f);
        character.controller.Play();
        callback?.Invoke(1);
    }

}
