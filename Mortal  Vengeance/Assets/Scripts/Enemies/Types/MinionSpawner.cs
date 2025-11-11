using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : EnemyBase
{
    [SerializeField] private Transform spawnPoint ;
    [SerializeField] private float spawnRate = 0.8f;
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private int initialPoolSize = 5;


    private float spawnTimer = 0f;
    private Queue<GameObject> minionPool = new Queue<GameObject>();
    protected override void Start()
    {
        base.Start();

        initPool(initialPoolSize);
    }

    private void initPool(int initialPoolSize)
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject minion = Instantiate(minionPrefab, transform.position, Quaternion.identity);


            if (minion.TryGetComponent<SuicideMinion>(out var minionScript))
            {
                minionScript.SetSpawner(this);
            }

            minion.SetActive(false);
            minionPool.Enqueue(minion);
        }
    }

   
    private GameObject GetMinion()
        {
            if (minionPool.Count == 0)
            {
                Debug.LogWarning("Pool-ul a rămas fără minioni! Creștem Pool-ul.");
                initPool(1);
            }

            // 1. Scoatem minionul din pool
            GameObject minionToSpawn = minionPool.Dequeue();

            // 2. Îl activăm și îl repoziționăm
            minionToSpawn.transform.position = spawnPoint.position;
            minionToSpawn.transform.rotation = Quaternion.identity;
            minionToSpawn.SetActive(true);

            return minionToSpawn;
        }

    public void ReturnMinionToPool(GameObject minion)
    {
        // 1. Dezactivăm obiectul
        minion.SetActive(false);

        // 2. Îl punem înapoi în coadă
        minionPool.Enqueue(minion);
    }

    // Modificarea funcției UpdateAttack()
    protected override void UpdateAttack()
    {
        // 1. Verificăm limita (Pool-ul este gol? Sau verifici o altă limită?)
        // Dacă vrei să menții limita maxMinions, o vei implementa cu o altă variabilă contor.
        // Dar pentru pooling simplu, ne concentrăm pe timer.

        if (agent != null)
        {
            agent.isStopped = true;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;

            // Verificăm dacă pool-ul are minioni disponibili
            if (minionPool.Count > 0)
            {
                GetMinion();
            }
        }
    }






}
