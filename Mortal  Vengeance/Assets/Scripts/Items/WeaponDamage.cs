using UnityEngine;

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
