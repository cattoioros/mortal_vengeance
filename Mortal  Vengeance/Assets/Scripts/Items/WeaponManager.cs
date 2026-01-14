using UnityEngine;

// Manages equipping and unequipping weapons for the player
public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolder;
    private GameObject currentWeapon;
    private PlayerAttack playerAttack;

    private void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
    }

    public void EquipWeapon(WeaponItemData weaponData)
    {
        if (weaponData == null || weaponData.equippedPrefab == null)
            return;

        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(
            weaponData.equippedPrefab,
            weaponHolder
        );

        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;

        Collider weaponCollider = currentWeapon.GetComponent<Collider>();
        if (weaponCollider == null)
        {
            weaponCollider = currentWeapon.AddComponent<BoxCollider>();
        }
        weaponCollider.isTrigger = true;
        weaponCollider.enabled = false;

        HitboxTrigger trigger = currentWeapon.GetComponent<HitboxTrigger>();
        if (trigger == null)
        {
            trigger = currentWeapon.AddComponent<HitboxTrigger>();
        }
        trigger.playerAttack = playerAttack;

        if (playerAttack != null)
        {
            playerAttack.hitbox = weaponCollider;
        }
    }

    public void Unequip()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        if (playerAttack != null)
        {
            playerAttack.hitbox = null;
        }
    }
}