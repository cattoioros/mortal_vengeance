using UnityEngine;
using UnityEngine.UI;


//visual part of the drag and drop system for inventory
public class InventoryDragHandler : MonoBehaviour
{
    public static InventoryDragHandler Instance; //singleton instance

    public Image dragIcon;

    private ItemData draggedItem;
    private InventorySlot sourceSlot;

    private void Awake()
    {
        Instance = this;
        dragIcon.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (dragIcon.gameObject.activeSelf)
        {
            dragIcon.transform.position = Input.mousePosition;//follow the mouse
        }
    }

    //starts dragging an item; save the item and slot
    public void StartDrag(ItemData item, InventorySlot slot)
    {
        draggedItem = item;
        sourceSlot = slot;

        dragIcon.sprite = item.icon;
        dragIcon.color = Color.white;
        dragIcon.gameObject.SetActive(true);
    }

    //stops dragging an item; clear the item and slot
    public void StopDrag()
    {
        dragIcon.gameObject.SetActive(false);
        draggedItem = null;
        sourceSlot = null;
    }

    //checks if there is an active drag operation
    public bool IsDragging => draggedItem != null;

    //gets the source slot of the dragged item
    public InventorySlot SourceSlot => sourceSlot;
}
