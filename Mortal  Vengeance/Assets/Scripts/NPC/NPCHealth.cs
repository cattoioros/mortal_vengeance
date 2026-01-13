using UnityEngine;
using Interfaces;

public class NPCHealth : MonoBehaviour, IsDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    private Animator animator;
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
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

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (col != null)
            col.enabled = false;

        animator.SetTrigger("Die");

        Destroy(gameObject, 2.5f);
    }
}
