using UnityEngine;


//inventory logic
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance; //singleton instance

    private InventorySlot selectedSlot;

    private void Awake()
    {
        Instance = this;
    }

    //called when a slot is clicked
    public void OnSlotClicked(InventorySlot clickedSlot)
    {
        //no slot selected yet
        if (selectedSlot == null)
        {
            if (clickedSlot.currentItem == null)
                return;

            selectedSlot = clickedSlot; //select the clicked slot
            Debug.Log("Selected slot " + clickedSlot.index);
            return;
        }

        //clicked the same slot again, deselect
        if (selectedSlot == clickedSlot)
        {
            selectedSlot = null;
            return;
        }

        //swap items between selected slot and clicked slot
        SwapItems(selectedSlot, clickedSlot);
        selectedSlot = null;
    }


    //called when an item is dropped onto a slot
    public void OnSlotDropped(InventorySlot from, InventorySlot to)
    {
        if (from == to) return;
        if (from.currentItem == null) return;

        ItemData draggedItem = from.currentItem;

        //equipment logic 
        if (to.slotType == InventorySlot.SlotType.Equipment && to.index == 1)
        {
            if (draggedItem is WeaponItemData weapon)
            {
                WeaponManager wm = FindObjectOfType<WeaponManager>();
                if (wm != null)
                    wm.EquipWeapon(weapon);
            }
            else if (draggedItem is ConsumableItemData consumable)
            {
                ConsumableManager cm = FindObjectOfType<ConsumableManager>();
                if (cm != null)
                    cm.EquipConsumable(consumable);
            }
        }


        //unequip logic
        if (from.slotType == InventorySlot.SlotType.Equipment && from.index == 1)
        {
            WeaponManager wm = FindObjectOfType<WeaponManager>();
            if (wm != null)
                wm.Unequip();

            ConsumableManager cm = FindObjectOfType<ConsumableManager>();
            if (cm != null)
                cm.Unequip();
        }



        // ITEM UI 
        to.SetItem(draggedItem);
        from.Clear();

        InventoryDragHandler.Instance.StopDrag();

        Debug.Log($"Moved item from slot {from.index} to slot {to.index}");
    }


    //swaps items between two slots
    private void SwapItems(InventorySlot a, InventorySlot b)
    {
        ItemData temp = a.currentItem;

        a.SetItem(b.currentItem);
        b.SetItem(temp);

        Debug.Log($"Swapped slot {a.index} with slot {b.index}");
    }
}
