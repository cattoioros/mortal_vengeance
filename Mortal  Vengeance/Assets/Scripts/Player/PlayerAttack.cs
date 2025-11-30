using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Interfaces;


public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int light_damage = 20;
    public float light_attackCooldown = 0.5f;
    public int heavy_damage = 40;
    public float heavy_attackCooldown = 1.2f;
    private float nextLightAttackTime = 0f;
    private float nextHeavyAttackTime = 0f;

    private int currentDamage = 0;


    public Collider hitbox; // collider pe sabie sau mana jucatorului


        private void Start()
    {
        hitbox.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryLightAttack();
        }
        if (Input.GetMouseButtonDown(1))
        {
            TryHeavyAttack();
        }

    }
    IEnumerator ActivateHitbox()
    {
        Debug.Log("Hitbox ON");
        hitbox.enabled = true;

        yield return new WaitForSeconds(0.1f);

        hitbox.enabled = false;
        Debug.Log("Hitbox OFF");
    }

    void TryLightAttack()
    {
    Debug.Log("Incerc sa atac... Time.time = " + Time.time + 
                  ", nextAttackTime = " + nextLightAttackTime);

        // Check cooldown
        if (Time.time < nextLightAttackTime)
        {
            Debug.Log("Nu pot ataca INCA! Cooldown activ.");
            return;
        }

        nextLightAttackTime = Time.time + light_attackCooldown;

        Debug.Log("ATAC LIGHT PORNIT! Cooldown pana la secunda: " + nextLightAttackTime);
        currentDamage = light_damage;

        StartCoroutine(ActivateHitbox());
    }

    void TryHeavyAttack()
    {
    Debug.Log("Incerc sa atac... Time.time = " + Time.time + 
                  ", nextAttackTime = " + nextHeavyAttackTime);

        // Check cooldown
        if (Time.time < nextHeavyAttackTime)
        {
            Debug.Log("Nu pot ataca INCA! Cooldown activ.");
            return;
        }

        nextHeavyAttackTime = Time.time + heavy_attackCooldown;

        Debug.Log("ATAC HEAVY PORNIT! Cooldown pana la secunda: " + nextHeavyAttackTime);
        currentDamage=heavy_damage;

        StartCoroutine(ActivateHitbox());
    }


        public void HandleHit(Collider other)
    {
        Debug.Log("Ceva a intrat in hitbox: " + other.name);

        IsDamageable dmg = other.GetComponent<IsDamageable>();

        if (dmg != null)
        {
            Debug.Log("Lovesc " + other.name + " cu " + currentDamage + " dmg!");
            dmg.TakeDamage(currentDamage);
        }
        else
        {
            Debug.Log(other.name + " NU are IDamageable!");
        }
    }
}




