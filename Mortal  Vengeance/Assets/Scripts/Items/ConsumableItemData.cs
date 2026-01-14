using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable")]

// Data class for consumable items like potions
public class ConsumableItemData : ItemData
{
    [Header("Consumable Stats")]
    public int healAmount;

    [Header("Prefabs")]
    public GameObject equippedPrefab;
}
