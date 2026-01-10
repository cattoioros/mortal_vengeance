using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolder;
    private GameObject currentWeapon;

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
    }

    public void Unequip()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }
    }
}
