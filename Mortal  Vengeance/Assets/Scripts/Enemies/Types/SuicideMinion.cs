using Unity.VisualScripting;
using UnityEngine;

public class SuicideMinion : EnemyBase
{

    [SerializeField]private float explosionRadius = 3f;
    [SerializeField]private int explosionDmg= 50;


    private MinionSpawner mySpawner;

    public void SetSpawner(MinionSpawner spawner)
    {
        mySpawner = spawner;
        }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Die();
        }
    }


    protected override void Die()
    {

        Explode();

        if (mySpawner != null)
        {
            mySpawner.ReturnMinionToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        Collider[] hitObjects = Physics.OverlapSphere(
            transform.position,
            explosionRadius);


        foreach (Collider collider in hitObjects)
        {
            /*if (collider.TryGetComponent<Player>(out var player))
            {
                // Aplică daunele exploziei
                player.TakeDamage(explosionDmg);
            }
            */

            if (collider.TryGetComponent<EnemyBase>(out var enemy))
            {
                // Asigură-te că nu aplici daune Minionului care explodează
                if (enemy.gameObject != gameObject)
                {
                    // Aplicăm daunele, de exemplu, la jumătate din valoarea exploziei
                    enemy.TakeDamage(explosionDmg / 2);
                }
            }
        }


    }

    protected override void UpdateChase()
    {
        if (agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
        }
    }


  
}
