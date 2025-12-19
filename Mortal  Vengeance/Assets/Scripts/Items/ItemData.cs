using UnityEngine;

public enum ItemType
{
    Test,
    Weapon,
    Armor,
    Consumable
}

[CreateAssetMenu(menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
}
