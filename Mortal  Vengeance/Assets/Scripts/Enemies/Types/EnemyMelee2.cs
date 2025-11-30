using UnityEngine;

public class EnemyMelee2 : EnemyBase
{
    private float lastAttackTime;


    protected override void AttackLogic()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = transform.forward;
            float rayLength = attackRange; 

            RaycastHit hit; 


            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength))
            {


                if (hit.collider.TryGetComponent<PlayerHealthManager>(out var playerHealth))
                {
                    playerHealth.TakeDamage(baseDmg);
                }
            }
            Debug.Log("Atac melee linie");
        }

       
    }
}



