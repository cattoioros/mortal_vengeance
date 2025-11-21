using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Panels")]
    public Transform equipmentPanel;  //panel with slots for armor and weapon that the player is currently using(5 slots)
    public Transform inventoryGrid;  //panel with slots for all items the player is carrying(27 slots)
    public Transform hotbarGrid;  //panel with slots for quick access to items(9 slots)

    [Header("Prefabs")]
    public GameObject slotPrefab;  //standard slot prefab to instantiate slots from

    void Start()
    {
        GenerateEquipmentSlot();
        GenerateInventorySlots();
        GenerateHotbarSlots();
    }

    //genretes 5 slotes for equipment
    void GenerateEquipmentSlot()
    {
        //Slots order: 0.Helmet, 1.Armor, 2.Legs, 3.Boots, 4.Weapon

        for(int i = 0; i < 5; i++)
        {
            //create a slot for EquipmentPanel
            GameObject slot = Instantiate(slotPrefab, equipmentPanel);

            //get InventorySlot component from the instantiated slot
            InventorySlot slotComp = slot.GetComponent<InventorySlot>();

            slotComp.index = i;
            slotComp.slotType = InventorySlot.SlotType.Equipment;

            slotComp.Refresh();
        }

    }

    //generates 27 slots for inventory
    void GenerateInventorySlots()
    {
        for (int i = 0; i < 27; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryGrid);

            InventorySlot slotComp = slot.GetComponent<InventorySlot>();
            slotComp.index = i;
            slotComp.slotType = InventorySlot.SlotType.Inventory;

            slotComp.Refresh();
        }
    }

    //generates 9 slots for hotbar
    void GenerateHotbarSlots()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject slot = Instantiate(slotPrefab, hotbarGrid);

            InventorySlot slotComp = slot.GetComponent<InventorySlot>();
            slotComp.index = i;
            slotComp.slotType = InventorySlot.SlotType.Hotbar;

            slotComp.Refresh();
        }
    }
}
