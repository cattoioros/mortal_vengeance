using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MinionSpawner : EnemyBase
{
    [SerializeField] private Transform spawnPoint ;
    [SerializeField] private float spawnRate = 1.5f;
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private int initialPoolSize = 5;


    [Header("Audio")]
    public SpawnerAudioController audioController;


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

            //Minion to be spawned out of the pool
            GameObject minionToSpawn = minionPool.Dequeue();

            //Positioning the minion 
            minionToSpawn.transform.position = spawnPoint.position;
            minionToSpawn.transform.rotation = Quaternion.identity;
            minionToSpawn.SetActive(true);

            return minionToSpawn;
        }

    public void ReturnMinionToPool(GameObject minion)
    {
        //Deactivating the minion and putting it back in the pool
        minion.SetActive(false);
        minionPool.Enqueue(minion);
    }


    public void spawnMinion()
    {
        // If there are no more minions in the pool, we create more
            if (minionPool.Count > 0)
            {
                GetMinion();
            }
            else
            {
                initPool(1);
            }

        if (audioController != null)
        {
            audioController.playSpawnMinion();
        }
    }

    protected override void UpdateAttack()
    {
        if(agent != null) agent.isStopped = true;

        animator.SetFloat("Speed", 0);
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
            if (animator != null) animator.SetTrigger("TriggerAttack");


        }
    }

    protected override void Die()
    {
        base.Die();

        if (audioController != null)
        {
            audioController.playDeathSound();
        }
    }






}
