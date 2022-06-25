using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class InventorySlotUI : MonoBehaviour
{
    public int x;
    public int y;

    public bool isHotBar;

    public void TakeItem()
    {
        var inventory = World.player.inventory;
        var slot = (isHotBar) ? inventory.slotsHotbar[x] : inventory.slots[x, y];

        if (World.tempSlot != null)
        {
            if (slot == null)
            {
                PushSlot(inventory, World.tempSlot);
                World.tempSlot = null;
                return;
            }

            if (slot.item.name == World.tempSlot.item.name)
            {
                AddToSlot(inventory, World.tempSlot);
                World.tempSlot = null;
                return;
            }
            
            PushSlot(inventory, World.tempSlot);
            World.tempSlot = slot;
            return;
        }
        
        World.tempSlot = slot;
        ClearSlot(inventory);
    }

    private void ClearSlot(Inventory inventory)
    {
        if (isHotBar)
            inventory.slotsHotbar[x] = null;
        else
            inventory.slots[x, y] = null;
    }
    
    private void PushSlot(Inventory inventory, InventorySlot slot)
    {
        if (isHotBar)
            inventory.slotsHotbar[x] = slot;
        else
            inventory.slots[x, y] = slot;
    }
    
    private void AddToSlot(Inventory inventory, InventorySlot slot)
    {
        if (isHotBar)
            inventory.slotsHotbar[x].quantity += slot.quantity;
        else
            inventory.slots[x, y].quantity += slot.quantity;
    }
}
