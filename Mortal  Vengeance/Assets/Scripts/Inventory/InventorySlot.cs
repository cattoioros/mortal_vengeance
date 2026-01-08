using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour,
    IPointerClickHandler, //click slot
    IBeginDragHandler, //start drag
    IDragHandler, //dragging
    IEndDragHandler, //end drag
    IDropHandler  //drop on slot
{
    public enum SlotType
    {
        Inventory,
        Hotbar,
        Equipment
    }

    public int index;  //slot index in inventory/hotbar/equipment
    public SlotType slotType;
    public Image icon;  //icon image for the item in this slot

    public ItemData currentItem; //the item currently in this slot

    // Sets the item in this slot and updates the icon
    public void SetItem(ItemData item)
    {
        currentItem = item;

        if (item == null)
        {
            icon.gameObject.SetActive(false);
            return;
        }

        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        icon.color = Color.white;
    }

    // Clears the slot
    public void Clear()
    {
        currentItem = null;
        icon.gameObject.SetActive(false);
    }

    // Handle pointer click event
    public void OnPointerClick(PointerEventData eventData)
    {
        InventorySystem.Instance.OnSlotClicked(this); //sends the event to InventorySystem
    }

    // Handle begin drag event
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        InventoryDragHandler.Instance.StartDrag(currentItem, this);
        icon.gameObject.SetActive(false); //hide icon in slot while dragging
    }

    //keep dragging
    public void OnDrag(PointerEventData eventData)
    {
        //the icon is dragged by InventoryDragHandler, so nothing needed here   
    }

    // Handle end drag event
    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryDragHandler.Instance.StopDrag();
    }


    //drop on this slot
    public void OnDrop(PointerEventData eventData)
    {
        if (!InventoryDragHandler.Instance.IsDragging) return;

        InventorySystem.Instance.OnSlotDropped(InventoryDragHandler.Instance.SourceSlot,this);
    }


    public bool IsEmpty()
    {
        return currentItem == null;
    }

}
