using UnityEngine;
using Interfaces;
using System.Collections;

// system for NPC health and damage handling
public class NPCHealth : MonoBehaviour, IsDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;


    [Header("Potion Drop")]
    public GameObject[] potionPickupPrefabs;
    public int minPotionDrop = 1;
    public int maxPotionDrop = 2;



    void Start()
    {
        currentHealth = maxHealth;
    }

    
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"NPC {gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        NPCMovement movement = GetComponent<NPCMovement>();
        if (movement != null)
            movement.enabled = false;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Wait for a moment before spawning potions
        yield return new WaitForSeconds(1f);

        SpawnPotions();

        Destroy(gameObject);
    }


    private void SpawnPotions()
    {
        // Determine random number of potions to drop
        int dropCount = Random.Range(minPotionDrop, maxPotionDrop + 1);

        for (int i = 0; i < dropCount; i++)
        {
            // Random offset around NPC position
            Vector3 offset = Random.insideUnitSphere;
            offset.y = 0.5f;

            // Select random potion prefab
            GameObject prefab = potionPickupPrefabs[Random.Range(0, potionPickupPrefabs.Length)];

            // Instantiate potion pickup
            Instantiate(
                prefab,
                transform.position + offset,
                Quaternion.identity
            );
        }
    }


}
