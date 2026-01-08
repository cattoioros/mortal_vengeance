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

    void Start()
    {
        if (debugTestItem != null)
        {
            AddItem(debugTestItem);
        }
    }

    //called before Start()
    void Awake()
    {
        GenerateEquipmentSlots();
        GenerateInventorySlots();
        GenerateHotbarSlots();
    }

    // create 5 slots for equipment panel
    void GenerateEquipmentSlots()
    {
        for (int i = 0; i < 5; i++)
        {
            InventorySlot slot = CreateSlot(equipmentPanel, i, InventorySlot.SlotType.Equipment);//the slot is created
            slot.Clear(); //slot is empty at the begining
        }
    }

    //create 27 slots for inventory panel
    void GenerateInventorySlots()
    {
        Debug.Log("GenerateInventorySlots CALLED");

        for (int i = 0; i < 27; i++)
        {
            InventorySlot slot = CreateSlot(inventoryGrid,i,InventorySlot.SlotType.Inventory
            );

            slot.Clear();

          
        }
    }


    //create 9 slots for hotbar panel
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

    
    //useless for now
    public InventorySlot[] GetInventorySlots()
    {
        return inventorySlots;
    }


    public bool AddItem(ItemData item)
    {
        // search for an empty slot in the inventory grid
        foreach (Transform child in inventoryGrid)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.IsEmpty())
            {
                slot.SetItem(item);
                return true;
            }
        }

        Debug.Log("Inventory full!");
        return false;
    }


}
