using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    private InventorySlot[] hotbarSlots;
    private int selectedIndex = -1;

    private WeaponManager weaponManager;
    private ConsumableManager consumableManager;

    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        consumableManager = GetComponent<ConsumableManager>();

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
        EquipFromSlot(slot);
    }

    void EquipFromSlot(InventorySlot slot)
    {
        if (slot.currentItem == null)
        {
            weaponManager?.Unequip();
            consumableManager?.Unequip();
            return;
        }

        if (slot.currentItem is WeaponItemData weapon)
        {
            consumableManager?.Unequip();
            weaponManager?.EquipWeapon(weapon);
        }
        else if (slot.currentItem is ConsumableItemData consumable)
        {
            weaponManager?.Unequip();
            consumableManager?.EquipConsumable(consumable);
        }
    }
}
