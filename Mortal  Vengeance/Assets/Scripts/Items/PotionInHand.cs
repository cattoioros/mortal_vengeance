using UnityEngine;

public class PotionInHand : MonoBehaviour
{
    public int healAmount;
    private bool used = false;

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
