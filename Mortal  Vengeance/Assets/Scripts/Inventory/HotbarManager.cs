using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    private InventorySlot[] hotbarSlots;
    private int selectedIndex = -1;

    void Start()
    {
        InventoryUIManager ui = FindObjectOfType<InventoryUIManager>();
        if (ui != null)
        {
            hotbarSlots = ui.hotbarSlots;
        }
    }

    void Update()
    {
        if (hotbarSlots == null) return;

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }
    }

    void SelectSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length)
            return;

        
        if (selectedIndex == index)
            return;

        selectedIndex = index;

        InventorySlot slot = hotbarSlots[index];

       
        InventorySystem.Instance?.EquipFromHotbar(slot);
    }
}
