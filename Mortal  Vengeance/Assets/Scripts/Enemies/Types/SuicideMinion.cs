using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SuicideMinion : EnemyBase
{
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDmg = 20;
    [SerializeField] private float explosionTriggerRadius = 1f;
    [SerializeField] private float explosionDelay = 1f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Material redMaterial;

    private bool explosionStarted = false;
    private MinionSpawner mySpawner;

   
    private SkinnedMeshRenderer skeletonRenderer;
    private Material originalMaterial;

    protected override void Start()
    {
        base.Start();

        skeletonRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (skeletonRenderer != null)
        {
            originalMaterial = skeletonRenderer.material;
        }
    }

    public void SetSpawner(MinionSpawner spawner)
    {
        mySpawner = spawner;
    }

    protected override void Die()
    {
        StopAllCoroutines();
        explosionStarted = false;

        if (skeletonRenderer != null && originalMaterial != null)
        {
            skeletonRenderer.material = originalMaterial;
        }

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
        if (animator != null) animator.SetTrigger("TriggerExplode");
        if (agent != null) agent.isStopped = true;
        if (skeletonRenderer != null && redMaterial != null)
        {
            skeletonRenderer.material = redMaterial;
        }

        yield return new WaitForSeconds(explosionDelay);

        Collider[] hitObjects = Physics.OverlapSphere(
            transform.position,
            explosionRadius
        );

        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 2f);

        foreach (Collider collider in hitObjects)
        {
            if (collider.TryGetComponent<PlayerStatsManager>(out var playerHealth))
            {
                playerHealth.TakeDamage(explosionDmg);
            }

            if (collider.TryGetComponent<EnemyBase>(out var enemy))
            {
                // Ignore self
                if (enemy.gameObject == gameObject) continue;
                // Less dmg for  Spawner
                if (enemy.GetType() == typeof(MinionSpawner)) enemy.TakeDamage(explosionDmg / 2);

                enemy.TakeDamage(explosionDmg);
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
            if (distanceToPlayer <= explosionTriggerRadius)
            {
                if (agent != null)
                {
                    agent.isStopped = true;
                }
                explosionStarted = true;
                StartCoroutine(Explode());
            }
        }
    }
}