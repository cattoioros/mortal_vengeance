using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Panels")]
    public Transform equipmentPanel;   // 5 slots
    public Transform inventoryGrid;    // 27 slots
    public Transform hotbarGrid;       // 9 slots

    [Header("Prefabs")]
    public GameObject slotPrefab;

    private InventorySlot[] inventorySlots;
    public ItemData debugTestItem;


    void Awake()
    {
        GenerateEquipmentSlots();
        GenerateInventorySlots();
        GenerateHotbarSlots();
    }

    void GenerateEquipmentSlots()
    {
        for (int i = 0; i < 5; i++)
        {
            InventorySlot slot = CreateSlot(equipmentPanel, i, InventorySlot.SlotType.Equipment);
            slot.Clear();
        }
    }

    void GenerateInventorySlots()
    {
        Debug.Log("GenerateInventorySlots CALLED");

        for (int i = 0; i < 27; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryGrid);

            InventorySlot slotComp = slot.GetComponent<InventorySlot>();

            if (slotComp == null)
            {
                Debug.LogError("InventorySlot component MISSING on prefab!");
                continue;
            }

            slotComp.index = i;
            slotComp.slotType = InventorySlot.SlotType.Inventory;
            slotComp.Clear();

            
            if (i == 0 && debugTestItem != null)
            {
                Debug.Log("Calling SetItem on slot 0");
                slotComp.SetItem(debugTestItem);
            }
        }
    }


    void GenerateHotbarSlots()
    {
        for (int i = 0; i < 9; i++)
        {
            InventorySlot slot = CreateSlot(hotbarGrid, i, InventorySlot.SlotType.Hotbar);
            slot.Clear();
        }
    }

    InventorySlot CreateSlot(Transform parent, int index, InventorySlot.SlotType type)
    {
        GameObject go = Instantiate(slotPrefab, parent);
        InventorySlot slot = go.GetComponent<InventorySlot>();

        slot.index = index;
        slot.slotType = type;

        return slot;
    }

    
    public InventorySlot[] GetInventorySlots()
    {
        return inventorySlots;
    }

    public void DebugAddItemToFirstSlot(ItemData item)
    {
        InventorySlot[] slots = inventoryGrid.GetComponentsInChildren<InventorySlot>();

        if (slots.Length > 0 && item != null)
        {
            slots[0].SetItem(item);
        }
    }

}
