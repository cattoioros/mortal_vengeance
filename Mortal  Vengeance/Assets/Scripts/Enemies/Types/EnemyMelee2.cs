using NUnit.Framework.Interfaces;
using UnityEngine;

public class EnemyMelee2 : EnemyBase
{
    private float lastAttackTime;

    [Header("Audio")]
    public MeleeAudioController audioController;

    public void AttackDmg()
    {
        float lowerHeight = 1.2f; 
        float radius = 0.3f;      

        Vector3 origin = transform.position + Vector3.up * lowerHeight;

        RaycastHit hit;

        if (Physics.SphereCast(origin, radius, transform.forward, out hit, attackRange))
        {
            if (hit.collider.TryGetComponent<PlayerStatsManager>(out var playerHealth))
            {
                playerHealth.TakeDamage(baseDmg);
                Debug.Log($"{gameObject.name} a lovit jucatorul la nivelul pieptului!");
            }
        }

    }



    public void EndAttack()
    {
        isAttacking = false;
    }

    public void AttackAudio()
    {
        if (audioController != null)
        {
            audioController.playSwordAttack();
        }
    }


    protected override void AttackLogic()
    {

        if (isAttacking) return;


        if (Time.time > lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            agent.isStopped = true;
            lastAttackTime = Time.time;

            if (animator != null)
            {
                animator.SetTrigger("TriggerAttack");
                
            }

            


        }
    }
}



