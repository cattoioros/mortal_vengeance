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



    public PlayerData GetPlayerData()
    {
        PlayerData data = new()
        {
            health = currentHealth,
            mana = currentMana,

            level = stats.level,
            experience = stats.experience,

            position = new float[]
            {
                transform.position.x,
                transform.position.y,
                transform.position.z
            }
        };

        return data;
    }

    public void ApplyPlayerData(PlayerData data)
    {
        currentHealth = data.health;
        currentMana = data.mana;

        stats.level = data.level;
        stats.experience = data.experience;

        transform.position = new Vector3(
            data.position[0],
            data.position[1],
            data.position[2]
        );
    }

}


