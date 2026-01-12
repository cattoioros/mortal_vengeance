using UnityEngine;

public class HitboxTrigger : MonoBehaviour
{
    public PlayerAttack playerAttack; 

    private void OnTriggerEnter(Collider other)
    
    {

        
        if (other.CompareTag("Player")) return;
        
        if(playerAttack != null)
        {

            playerAttack.HandleHit(other);
        }
    }
}