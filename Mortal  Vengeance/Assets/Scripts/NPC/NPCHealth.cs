using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;    
    }

    // Test damage with space key to demonstrate functionality
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed - NPC will take damage.");
            TakeDamage(25f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("NPC "+gameObject.name+" took "+damage+" damage. Current health: "+currentHealth);
        if (currentHealth < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("NPC "+gameObject.name+" has died.");
        Destroy(gameObject);
    }


}
