using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public PlayerStats stats;

    private float currentHealth;
    private float currentMana;

    /*
    void Start()
    {
        currentHealth = stats.maxHealth;
        currentMana = stats.maxMana;
    }
    */

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, stats.maxHealth);
    }


    public void UseMana(float amount)
    {
        currentMana = Mathf.Max(currentMana - amount, 0);
    }

    private void Die()
    {
        Debug.Log("Ai murit");
    }

}


