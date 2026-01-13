using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public Transform itemHolder;

    private PotionInHand currentPotion;
    private GameObject currentItem;
    private PlayerStatsManager statsManager;
    private ItemData equippedConsumableData;


    void Start()
    {
        statsManager = GetComponent<PlayerStatsManager>();
    }

    public void EquipConsumable(ConsumableItemData data)
    {
        Unequip();

        currentItem = Instantiate(data.equippedPrefab, itemHolder);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.transform.localScale = Vector3.one;

        currentPotion = currentItem.GetComponent<PotionInHand>();
        if (currentPotion != null)
        {
            equippedConsumableData = data;
            currentPotion.healAmount = data.healAmount;
        }
    }

    void Update()
    {
        if (currentPotion != null && Input.GetKeyDown(KeyCode.Q))
        {
            currentPotion.Use(statsManager);
            InventorySystem.Instance?.ConsumeEquippedItem();
            Unequip();
        }
    }


    public void Unequip()
    {
        if (currentItem != null)
            Destroy(currentItem);

        currentItem = null;
        currentPotion = null;
    }
}
