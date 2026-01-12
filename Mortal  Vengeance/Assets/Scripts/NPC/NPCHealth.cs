using UnityEngine;
using Interfaces;

public class NPCHealth : MonoBehaviour, IsDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"NPC {gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"NPC {gameObject.name} has died.");
        Destroy(gameObject);
    }
}
