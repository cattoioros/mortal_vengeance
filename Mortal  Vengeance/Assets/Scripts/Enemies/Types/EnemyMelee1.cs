using UnityEngine;

public class EnemyMelee1 : EnemyBase
{
    private float lastAttackTime;


    protected override void AttackLogic()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {

            lastAttackTime = Time.time;

            Vector3 playerDirection = playerTarget.position - transform.position;

            float unghiAtac = Vector3.Angle(transform.forward, playerDirection);

            if (unghiAtac < 30)
            {
                if (playerTarget.TryGetComponent<PlayerHealthManager>(out var playerHealth))
                {
                    playerHealth.TakeDamage(baseDmg);
                }

            }

        }

        Debug.Log("Atac melee wide");

    }
}



