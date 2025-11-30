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

            //Scoatem minionul din pool
            GameObject minionToSpawn = minionPool.Dequeue();

            //Activam minionul si il pozitionam
            minionToSpawn.transform.position = spawnPoint.position;
            minionToSpawn.transform.rotation = Quaternion.identity;
            minionToSpawn.SetActive(true);

            return minionToSpawn;
        }

    public void ReturnMinionToPool(GameObject minion)
    {
        //Dezactivam obiectul
        minion.SetActive(false);

        //Il punem inapoi in coada
        minionPool.Enqueue(minion);
    }

    protected override void UpdateAttack()
    {
        float PlayerDistance = Vector3.Distance(playerTarget.position, transform.position);

        if(PlayerDistance > attackRange)
        {
            currentState = EnemyState.Chase;
            if (agent != null) agent.isStopped = false;
            return;
        }

        if (agent != null)
        {
            agent.isStopped = true;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;

            // Verificam daca mai sunt minioni in pool
            if (minionPool.Count > 0)
            {
                GetMinion();
            }
        }
    }






}
