using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable")]
public class ConsumableItemData : ItemData
{
    [Header("Consumable Stats")]
    public int healAmount;

    [Header("Prefabs")]
    public GameObject equippedPrefab;
}
