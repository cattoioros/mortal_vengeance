using UnityEngine;

// Manages equipping and using consumable items like potions
public class ConsumableManager : MonoBehaviour
{
    public Transform itemHolder;

    private PotionInHand currentPotion;
    private GameObject currentItem;
    private PlayerStatsManager statsManager;
    private ItemData equippedConsumableData;


    void Start()
    {
        // Get reference to PlayerStatsManager for healing
        statsManager = GetComponent<PlayerStatsManager>();
    }

    // Equip a consumable item (e.g., potion) in the player's hand
    public void EquipConsumable(ConsumableItemData data)
    {
        Unequip();

        currentItem = Instantiate(data.equippedPrefab, itemHolder);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.transform.localScale = Vector3.one;

        // Get the PotionInHand component to set heal amount
        currentPotion = currentItem.GetComponent<PotionInHand>();
        if (currentPotion != null)
        {
            equippedConsumableData = data;
            currentPotion.healAmount = data.healAmount;
        }
    }

    
    void Update()
    {
        // Use the equipped consumable when Q is pressed
        if (currentPotion != null && Input.GetKeyDown(KeyCode.Q))
        {
            currentPotion.Use(statsManager);
            InventorySystem.Instance?.ConsumeEquippedItem();
            Unequip();
        }
    }


    // Unequip the currently equipped consumable item
    public void Unequip()
    {
        if (currentItem != null)
            Destroy(currentItem);

        currentItem = null;
        currentPotion = null;
    }
}
