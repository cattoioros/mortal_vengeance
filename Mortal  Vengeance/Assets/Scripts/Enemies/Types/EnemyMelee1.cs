using UnityEngine;

public class EnemyMelee1 : EnemyBase
{
    private float lastAttackTime;

    public void AttackDmg()
    {
        Vector3 playerDirection = playerTarget.position - transform.position;

        float unghiAtac = Vector3.Angle(transform.forward, playerDirection);

        if (unghiAtac < 30)
        {
            if (playerTarget.TryGetComponent<PlayerHealthManager>(out var playerHealth))
            {
                playerHealth.TakeDamage(baseDmg);
                Debug.Log("Am lovit");
            }

        }
    }

    public void AttackEnd()
    {
        isAttacking = false;
    }


    protected override void AttackLogic()
    {

        if (isAttacking) return;


        if (Time.time > lastAttackTime + attackCooldown)
        {

            lastAttackTime = Time.time;
            isAttacking = true;
            if(animator!=null)
            {

                animator.SetTrigger("TriggerAttack");
            }

        }

        Debug.Log("Atac melee wide");

    }
}



