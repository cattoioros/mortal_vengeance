using UnityEngine;

public class PotionInHand : MonoBehaviour
{
    public int healAmount;
    private bool used = false;

    // Method to use the potion
    public void Use(GameObject player)
    {
        if (used) return;
        used = true;

        PlayerStatsManager statsManager = player.GetComponent<PlayerStatsManager>();
        if (statsManager != null)
        {
            statsManager.Heal(healAmount);
        }
        else
        {
            Debug.LogError("PlayerStatsManager not found on Player!");
        }

        Destroy(gameObject);
    }
}
