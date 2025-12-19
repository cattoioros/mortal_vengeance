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
    public int index;

    [Header("UI")]
    public Image icon;

    private ItemData currentItem;

    public void SetItem(ItemData item)
    {
        currentItem = item;

        if (item == null)
        {
            icon.enabled = false;
            return;
        }

        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        icon.color = Color.white; 
        icon.enabled = true;
    }



    public void Clear()
    {
        currentItem = null;
        icon.enabled = false;
    }
}
