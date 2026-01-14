using UnityEngine;

/// Manages hotbar input and equips items from the hotbar
public class HotbarManager : MonoBehaviour
{
    private InventorySlot[] hotbarSlots;

    void Start()
    {
        // Get reference to hotbar slots from InventoryUIManager
        InventoryUIManager ui = FindObjectOfType<InventoryUIManager>();
        if (ui != null)
        {
            hotbarSlots = ui.hotbarSlots;
        }
    }

    void Update()
    {
        // Check for number key inputs to equip items from hotbar
        if (hotbarSlots == null) return;

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipSlot(i);
            }
        }
    }

    void EquipSlot(int index)
    {
        // Equip item from the specified hotbar slot
        if (index < 0 || index >= hotbarSlots.Length)
            return;

        InventorySlot slot = hotbarSlots[index];

        if (slot.currentItem == null)
            return;

        InventorySystem.Instance?.EquipFromHotbar(slot);
    }
}
