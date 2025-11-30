using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : EnemyBase
{
    private float lastAttackTime;
    [Header("Ranged Specific")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject arrowPrefab; 
    [SerializeField] private float speed = 10f;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] public int arrowDmg;



    private Queue<GameObject> arrowPool = new Queue<GameObject>();


    protected override void Start()
    {
        base.Start();

        initPool(initialPoolSize);
    }

    private void initPool(int initialPoolSize)
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

            arrow.SetActive(false);
            arrowPool.Enqueue(arrow);
        }
    }

    private GameObject GetArrow()
    {
        if (arrowPool.Count == 0)
        {
            Debug.LogWarning("Pool-ul a ramas fara sageti.");
            initPool(1);
        }

        //Scoatem sageata din pool
        GameObject arrowToSpawn = arrowPool.Dequeue();

        arrowToSpawn.transform.position = spawnPoint.position;
        arrowToSpawn.transform.rotation = Quaternion.identity;
        arrowToSpawn.SetActive(true);

        return arrowToSpawn;
    }


    public void ReturnArrowToPool(GameObject arrow)
    {
        arrow.SetActive(false);

        arrowPool.Enqueue(arrow);
    }

    protected override void AttackLogic()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {

            lastAttackTime = Time.time;
            GameObject readyArrow = GetArrow();
            Vector3 PlayerDirection = playerTarget.transform.position - spawnPoint.position;
            readyArrow.transform.rotation = Quaternion.LookRotation(PlayerDirection);


            Rigidbody rb = readyArrow.GetComponent<Rigidbody>();

            rb.linearVelocity = PlayerDirection.normalized * speed;

            Arrow arrowScript = readyArrow.GetComponent<Arrow>();

            arrowScript.setArcher(this);




        }
    }

}
