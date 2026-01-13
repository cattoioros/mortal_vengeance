using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    private InventorySlot[] hotbarSlots;

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
                EquipSlot(i);
            }
        }
    }

    void EquipSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length)
            return;

        InventorySlot slot = hotbarSlots[index];

        if (slot.currentItem == null)
            return;

        InventorySystem.Instance?.EquipFromHotbar(slot);
    }
}
