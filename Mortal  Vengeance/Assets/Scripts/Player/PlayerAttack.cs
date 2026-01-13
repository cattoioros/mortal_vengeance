using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using NUnit.Framework;

public class PlayerAttack : MonoBehaviour
{


    public bool isAttacking = false;
    
    [Header("Attack Damage")]
    public PlayerStats stats;

    public int light_damage ;
    public int heavy_damage ;



    [Header("Cooldowns")]
    public float light_attackCooldown = 0.6f;
    public float heavy_attackCooldown = 1.2f;
    
    [Header("Combo Settings")]
    public float comboResetTime = 5.0f; // Timpul pana la resetarea combo-ului
    private int currentAttackIndex = 0; // 0, 1, 2
    private float lastAttackTime;

    private float nextLightAttackTime = 0f;
    private float nextHeavyAttackTime = 0f;
    private int currentDamage = 0;


    private List<GameObject> enemiesHit = new List<GameObject>();


    [Header("References")]
    public Collider hitbox;
    [SerializeField] private Animator animator;
public void EndAttack()
{
    isAttacking = false;
}
public void StartAttack()
{
    isAttacking = true;
}

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        
        if (hitbox != null)
            hitbox.enabled = false;

        if (stats == null) 
            stats = GetComponent<PlayerStats>();

            
        isAttacking=false;

    }

void Update()
{
    if (currentAttackIndex != 0)
    {
        if (Time.time > lastAttackTime + comboResetTime)
        {
            Debug.Log("Combo Resetat după " + comboResetTime + " secunde de inactivitate.");
            currentAttackIndex = 0;
            animator.SetInteger("AttackIndex", 0);
        }
    }

    if (Input.GetMouseButtonDown(0)) TryLightAttack();
    if (Input.GetMouseButtonDown(1)) TryHeavyAttack();
}
void TryLightAttack()
{
    light_damage = (int)stats.attackPower;
    if (Time.time < nextLightAttackTime) return;

    lastAttackTime = Time.time; 
    nextLightAttackTime = Time.time + light_attackCooldown;
    
    currentDamage = light_damage;

    animator.SetInteger("AttackIndex", currentAttackIndex);
    animator.SetTrigger("LightAttack");

    Debug.Log("Atac pornit. Index: " + currentAttackIndex + ". Următorul reset la: " + (lastAttackTime + comboResetTime));

    currentAttackIndex++;
    if (currentAttackIndex >= 3) currentAttackIndex = 0;

    

    StopAllCoroutines();
    StartCoroutine(ActivateHitbox());
    
}

    void TryHeavyAttack()
    {
        heavy_damage=(int)stats.attackPower*2;
        if (Time.time < nextHeavyAttackTime) return;

        nextHeavyAttackTime = Time.time + heavy_attackCooldown;
        currentDamage = heavy_damage;

        // Resetam combo-ul cand dam un Heavy
        currentAttackIndex = 0;
        animator.SetTrigger("HeavyAttack");
        StopAllCoroutines();
        StartCoroutine(ActivateHitbox());
    
    }

    IEnumerator ActivateHitbox()
    {
        
        // Un mic delay pentru a lasa animatia sa porneasca inainte de a activa colliderul

        enemiesHit.Clear();

        yield return new WaitForSeconds(0.15f);


        if (hitbox != null)
        {
            hitbox.enabled = true;
            yield return new WaitForSeconds(1.5f); // Cat timp ramane atacul activ
            hitbox.enabled = false;
        }



        
    }

    public void HandleHit(Collider other)
    {


        if (enemiesHit.Contains(other.gameObject)) return;


        var dmg = other.GetComponent<Interfaces.IsDamageable>();

        if (dmg != null)
        {

            enemiesHit.Add(other.gameObject);

            Debug.Log("Lovesc " + other.name + " cu " + currentDamage + " dmg!");
            dmg.TakeDamage(currentDamage);
        }
    }
}
