using UnityEngine;

/// This script handles the behavior of a potion when it is held in the player's hand.
public class PotionInHand : MonoBehaviour
{
    public int healAmount;
    private bool used = false;

    /// Uses the potion to heal the player and destroys the potion object.
    public void Use(PlayerStatsManager statsManager)
    {
        if (used) return;
        used = true;

        if (statsManager != null)
        {
            statsManager.Heal(healAmount);
        }

        Destroy(gameObject);
    }
}
