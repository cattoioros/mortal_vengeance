using UnityEngine;


//inventory logic
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance; //singleton instance

    private InventorySlot selectedSlot;
    private InventorySlot equippedSlot;
    private InventorySlot equippedSourceSlot;
    private InventoryUIManager ui;
    private WeaponManager weaponManager;
    private ConsumableManager consumableManager;



    private void Awake()
    {
        Instance = this;
        ui = FindObjectOfType<InventoryUIManager>();
        weaponManager = FindObjectOfType<WeaponManager>();
        consumableManager = FindObjectOfType<ConsumableManager>();
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

        // EQUIP
        if (to.slotType == InventorySlot.SlotType.Equipment && to.index == 1)
        {
            if (draggedItem is WeaponItemData weapon)
            {
                weaponManager?.EquipWeapon(weapon);
                consumableManager?.Unequip();
            }
            else if (draggedItem is ConsumableItemData consumable)
            {
                consumableManager?.EquipConsumable(consumable);
                weaponManager?.Unequip();

                equippedSlot = to;
                equippedSourceSlot = from;
            }
        }

        // UNEQUIP
        if (from.slotType == InventorySlot.SlotType.Equipment && from.index == 1)
        {
            weaponManager?.Unequip();
            consumableManager?.Unequip();
        }

        // UI MOVE
        to.SetItem(draggedItem);
        from.Clear();

        InventoryDragHandler.Instance.StopDrag();
    }



    //swaps items between two slots
    private void SwapItems(InventorySlot a, InventorySlot b)
    {
        ItemData temp = a.currentItem;

        a.SetItem(b.currentItem);
        b.SetItem(temp);

        Debug.Log($"Swapped slot {a.index} with slot {b.index}");
    }

    public void EquipFromHotbar(InventorySlot hotbarSlot)
    {
        if (hotbarSlot == null || hotbarSlot.currentItem == null)
            return;

        if (ui == null || ui.equipmentPanel == null)
        {
            Debug.LogError("InventoryUIManager or equipmentPanel missing!");
            return;
        }

        InventorySlot equipmentSlot = null;

        foreach (Transform child in ui.equipmentPanel)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.index == 1)
            {
                equipmentSlot = slot;
                break;
            }
        }

        if (equipmentSlot == null)
            return;

        // SAFE SWAP
        ItemData previous = equipmentSlot.currentItem;

        equipmentSlot.SetItem(hotbarSlot.currentItem);
        hotbarSlot.Clear();

        if (previous != null)
        {
            hotbarSlot.SetItem(previous);
        }

        equippedSlot = equipmentSlot;
        equippedSourceSlot = hotbarSlot;

        if (equipmentSlot.currentItem is WeaponItemData weapon)
        {
            weaponManager?.EquipWeapon(weapon);
            consumableManager?.Unequip();
        }
        else if (equipmentSlot.currentItem is ConsumableItemData consumable)
        {
            consumableManager?.EquipConsumable(consumable);
            weaponManager?.Unequip();
        }
    }


    public void ConsumeEquippedItem()
    {
        if (equippedSlot == null)
            return;

        equippedSlot.Clear();

        if (equippedSourceSlot != null)
            equippedSourceSlot.Clear();

        equippedSlot = null;
        equippedSourceSlot = null;
    }



    public void RemoveItemEverywhere(ItemData item)
    {
        InventoryUIManager ui = FindObjectOfType<InventoryUIManager>();
        if (ui == null) return;

        // Equipment
        foreach (Transform child in ui.equipmentPanel)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.currentItem == item)
                slot.Clear();
        }

        // Hotbar
        foreach (Transform child in ui.hotbarGrid)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.currentItem == item)
                slot.Clear();
        }

        // Inventory
        foreach (Transform child in ui.inventoryGrid)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.currentItem == item)
                slot.Clear();
        }
    }



}
