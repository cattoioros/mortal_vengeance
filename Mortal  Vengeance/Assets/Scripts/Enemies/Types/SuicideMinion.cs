using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SuicideMinion : EnemyBase
{

    [SerializeField]private float explosionRadius = 3f;
    [SerializeField]private int explosionDmg= 50;
    [SerializeField]private float explosionTriggerRadius = 1.5f;
    [SerializeField]private float explosionDelay = 1f;
    private bool explosionStarted = false;


    private MinionSpawner mySpawner;

    public void SetSpawner(MinionSpawner spawner)
    {
        mySpawner = spawner;
        }

   

    protected override void Die()
    {

        StopAllCoroutines();

        explosionStarted = false;

        if (mySpawner != null)
        {
            mySpawner.ReturnMinionToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(explosionDelay);

        Collider[] hitObjects = Physics.OverlapSphere(
            transform.position,
            explosionRadius
            );

        foreach (Collider collider in hitObjects)
        {
            // Aplicam damage asupra player-ului
            /*if (collider.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(explosionDmg);
            }
            */

            if (collider.TryGetComponent<EnemyBase>(out var enemy))
            {
                // Ignore self
                if (enemy.gameObject == gameObject) continue;

                // Ignora Spawner-ul
                if (enemy.GetType() == typeof(MinionSpawner)) continue;

                enemy.TakeDamage(explosionDmg / 2);
            }
        }




        Die();


    }

    protected override void UpdateChase()
    {
        if (agent != null && !explosionStarted)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
        }

        if (playerTarget != null && !explosionStarted)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if(distanceToPlayer <= explosionTriggerRadius)
            {
                if(agent!= null)
                {
                    agent.isStopped = true;
                }

                explosionStarted = true;
                StartCoroutine(Explode());
            }
        }

       
    }


  
}
