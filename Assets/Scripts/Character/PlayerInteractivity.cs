using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractivity : CharacterComponent {

    public Interactive itemInHand;

    [SerializeField] private Interactive hoveredObject;

    public override void Init(Character character) {
        base.Init(character);
        if (itemInHand) {
            itemInHand.ResetObject();
        }
        character.model.animator.SetBool("crabInHands", false);
        character.model.animator.ResetTrigger("PickUp");
        character.model.animator.ResetTrigger("PickDown");
        character.model.animator.ResetTrigger("PickCrabUp");
        character.model.animator.ResetTrigger("PickCrabDown");
        itemInHand = null;
    }

    public bool crabIsInHands() {
        return itemInHand && itemInHand.isCrab;
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

    public void DropItem(bool all) {
        if (itemInHand == null || !(itemInHand is Item)) return;
        itemInHand = (itemInHand as Item).OnDrop(character);
        hoveredObject = itemInHand;
        if (all) {
            DropItem(all);
        }
    }

    public void TakeCrab(CrabController crab) {
        crab.SetInHands(character);
        itemInHand = crab.character;
        character.movement.InputSpeedDebuff(true);
        character.model.animator.SetBool("crabInHands", true);
    }

    public void PickupCrab(CrabController crab) {
        StartCoroutine(_PickupCrab(crab));
    }
    IEnumerator _PickupCrab(CrabController crab) {
        character.controller.Pause();
        character.movement.InputDirection((crab.transform.position - transform.position).SetY());
        character.model.animator.SetTrigger("PickCrabUp");
        yield return new WaitForSeconds(.3f);
        TakeCrab(crab);
        yield return new WaitForSeconds(.8f);
        character.controller.Play();
    }

    public void DropCrab() {
        if (itemInHand == null || itemInHand.data.type == InteractData.Type.Item) return;
        itemInHand.GetComponent<CrabController>().SetInHands(null);
        itemInHand = null;
        character.movement.InputSpeedDebuff(false);
        character.model.animator.SetBool("crabInHands", false);
    }
    public void PlaceCrab() {
        if (itemInHand == null || itemInHand.data.type == InteractData.Type.Item) return;
        StartCoroutine(_PlaceCrab());
    }
    IEnumerator _PlaceCrab() {
        character.controller.Pause();
        character.model.animator.SetTrigger("PickCrabDown");
        character.model.animator.SetBool("crabInHands", false);
        yield return new WaitForSeconds(.8f);
        DropCrab();
        yield return new WaitForSeconds(.3f);
        character.controller.Play();
    }

    private void UpdateHovered() {
        Interactive hovered;
        if (character.movement.isSprinting) {
            hovered = null;
        } else {
            hovered = character.radar.GetNearest(InteractData.Type.All, CharacterRadar.HandState.NotInHand);
            if (!hovered || !hovered.CanInteract(character)) {
                if (itemInHand && itemInHand.CanInteract(character)) {
                    hovered = itemInHand;
                } else {
                    hovered = null;
                }
            }
        }
        if (hoveredObject != hovered) {
            hoveredObject = hovered;
            if (hovered) {
                UIManager.main.SetItemHint(hovered.GetHint());
            } else {
                UIManager.main.SetItemHint(string.Empty);
            }
        }
    }

    private void FixedUpdate() {
        UpdateHovered();
    }

    public void InputInteract() {
        if (character.movement.isSprinting) return;
        if (hoveredObject) {
            hoveredObject.Interact(character);
        }
    }
    public void InputDrop() {
        if (character.movement.isSprinting) return;
        PlaceItem(true);
        PlaceCrab();
    }

    public void PickupItem(Item item) {
        StartCoroutine(_PickupItem(item));
    }
    IEnumerator _PickupItem(Item item) {
        character.controller.Pause();
        character.movement.InputDirection((item.transform.position - transform.position).SetY());
        character.model.animator.SetTrigger("PickUp");
        yield return new WaitForSeconds(.5f);
        TakeItem(item);
        yield return new WaitForSeconds(.6f);
        character.controller.Play();
    }

    public void PlaceItem(bool all = false) {
        if (!itemInHand || itemInHand.data.type != InteractData.Type.Item) return;
        StartCoroutine(_PlaceItem(all));
    }
    IEnumerator _PlaceItem(bool all) {
        character.controller.Pause();
        character.model.animator.SetTrigger("PickDown");
        yield return new WaitForSeconds(.4f);
        DropItem(all);
        yield return new WaitForSeconds(.7f);
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
