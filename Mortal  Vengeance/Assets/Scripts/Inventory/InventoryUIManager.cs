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

    [HideInInspector]
    public InventorySlot[] hotbarSlots;

    void Start()
    {
        
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
        hotbarSlots = new InventorySlot[9];
        for (int i = 0; i < 9; i++)
        {
            InventorySlot slot = CreateSlot(hotbarGrid, i, InventorySlot.SlotType.Hotbar);
            slot.Clear();
            hotbarSlots[i] = slot;
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
        //search for an empty slot in HOTBAR first
        foreach (Transform child in hotbarGrid)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.IsEmpty())
            {
                slot.SetItem(item);
                return true;
            }
        }

        //search for an empty slot in INVENTORY second
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
