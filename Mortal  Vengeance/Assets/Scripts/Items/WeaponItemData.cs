using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponItemData : ItemData
{
    [Header("Weapon Stats")]
    public int damage;
    public float attackSpeed;

    [Header("Prefabs")]
    public GameObject equippedPrefab;
}

