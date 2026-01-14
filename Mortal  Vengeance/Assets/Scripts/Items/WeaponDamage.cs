using UnityEngine;

// when the weapon collides with something, it notifies the PlayerAttack component to handle the hit
public class WeaponDamage : MonoBehaviour
{
    public PlayerAttack playerAttack;

    private void OnTriggerEnter(Collider other)
    {
        if (playerAttack == null)
            return;

        playerAttack.HandleHit(other);
    }
}
