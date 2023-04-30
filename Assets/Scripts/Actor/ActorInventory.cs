using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorInventory : ActorComponent {

    public CrabController crabInHand;
    public Item itemInHand;

    public List<ItemSlot> slots = new List<ItemSlot>();

    public int GetIndex(Item item) {
        for (int i = 0; i < slots.Count; i++) {
            if (item.data.name == slots[i].data.name) {
                return i;
            }
        }
        return -1;
    }

    public void AddItem(Item item) {
        var index = GetIndex(item);
        if (index > -1) {
            slots[index].count++;
        } else {
            slots.Add(new ItemSlot() { data = item.data, count = 1 });
        }
        item.OnTake(actor);
    }

    public void TakeInHand(int index) {
        if (index < slots.Count) {
            itemInHand = Instantiate(slots[index].data.prefab, actor.model.armPoint);
            itemInHand.OnTake(actor);
            slots.RemoveAt(index);
        }
    }

    public void DropItem() {
        if (itemInHand == null) return;
        itemInHand.transform.SetParent(null);
        itemInHand.OnDrop(actor);
        itemInHand = null;
    }

    public void TakeCrab(CrabController crab) {
        crabInHand = crab;
        crabInHand.SetInHands(actor);
    }

    public void DropCrab() {
        if (crabInHand == null) return;
        crabInHand.SetInHands(null);
        crabInHand = null;
    }


}
