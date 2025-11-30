using UnityEngine;

public class HitboxTrigger : MonoBehaviour
{
    public PlayerAttack playerAttack; // referință la scriptul de pe player

    private void OnTriggerEnter(Collider other)
    {
        if(playerAttack != null)
            playerAttack.HandleHit(other);
    }
}