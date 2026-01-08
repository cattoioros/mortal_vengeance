using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public enum SlotType
    {
        Inventory,
        Equipment,
        Hotbar
    }

    [Header("Slot Info")]
    public SlotType slotType;
    public int index;   //index of the slot in its respective panel

    [Header("UI Elements")]
    public Image icon;

    //is called when the slot must be visually updated
    public void Refresh()
    {
        icon.enabled = false; //for now, the icon is disabled because there are no items implemented yet
    }
}
