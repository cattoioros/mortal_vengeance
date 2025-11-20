using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;




public class PromisedBoss : EnemyBase
{
    [Header("Boss Specific")]
    [SerializeField]private float rangeAttackDistance;



    [Header("Meteors Ranged")]
    [SerializeField] private float meteorSpawnHeight = 20f;
    [SerializeField] private float meteorDelayTime = 3.0f;
    [SerializeField] private float meteorAoERadius = 3.5f;
    [SerializeField] private int nrMeteors = 5;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject meteorPrefab;

    private Queue<GameObject> meteorPool = new Queue<GameObject>();

    private enum AttackStart
    {
        None,
        Meteors,
        Charge,
        AOEDrag,
        ExplodeSlash,
        FrontFlip,
        DoubleSweep,
        RightSweep,

    }

    private AttackStart CurrentCombo = AttackStart.None;
    private int step = 0;


    private IEnumerator PerformGroundStomp()
    {
        Debug.Log("Boss: Incep Stomp-ul de la sol.");

        yield return new WaitForSeconds(0.5f);

        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, 1.5f); 
        foreach (var hit in hitPlayers)
        {
            if (hit.TryGetComponent<PlayerHealthManager>(out var playerHealth))
            {
                playerHealth.TakeDamage(baseDmg / 3); 
            }
        }
        Debug.Log("Boss: Daune mici aplicate. Trec in aer.");


        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator PerformMove(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t / duration);
            yield return null;
        }

        transform.position = targetPos;
    }


    IEnumerator PerformMeteors()
    {
        yield return PerformGroundStomp();

        Vector3 positionUp = new Vector3(transform.position.x, transform.position.y + meteorSpawnHeight, transform.position.z);


        yield return PerformMove(positionUp,2f);

        Debug.Log("Meteoriti spawning");

        Vector3 playerPos = playerTarget.position;

        List<GameObject> meteoritiActivi = new List<GameObject>();

        for (int i = 0; i < nrMeteors; i++)
        {
            float angle = i * Mathf.PI * 2 / nrMeteors;
            Vector3 spawnOffset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 2.5f;

            GameObject meteor = GetMeteor();

            meteor.transform.position = transform.position + spawnOffset;

            Meteor meteorScript = meteor.GetComponent<Meteor>();
            if (meteorScript != null)
            {
                meteorScript.SetMeteor(
                    playerPos, 
                    meteorDelayTime,
                    baseDmg * 2,
                    meteorAoERadius,
                    this
                );
                meteoritiActivi.Add(meteor);
            }


        }

        
        yield return new WaitForSeconds(meteorDelayTime);

        Vector3 positionDown = new Vector3(transform.position.x, transform.position.y - meteorSpawnHeight, transform.position.z);

        yield return PerformMove(positionDown, 2f);


        CurrentCombo = AttackStart.None;
        step = 0;


    }




    IEnumerator ExecuteMeteorsCombo()
    {
        yield return StartCoroutine(PerformMeteors());

        CurrentCombo = AttackStart.None;
        step = 0;
    }

    IEnumerator ExecuteChargeCombo()
    {
        //yield return StartCoroutine(PerformCharge());

        yield break;
        CurrentCombo = AttackStart.None;
        step = 0;
    }

    IEnumerator ExecuteExplodeSlashCombo()
    {
        //yield return StartCoroutine(PerformExplodeSlash());

        yield break;
        CurrentCombo = AttackStart.None;
        step = 0;
    }

    IEnumerator ExecuteFrontFlipCombo()
    {
        //yield return StartCoroutine(PerformFrontFlip());
        yield return new WaitForSeconds(1);

        if (Random.value < 0.5f)
        {
            CurrentCombo = AttackStart.None;
            step = 0;
            yield break;
        }
        else
        {
            //yield return StartCoroutine(PerformFrontFlip());

        }

        //ajutor pentru player sa atace dupa combo
        yield return new WaitForSeconds(2);

        CurrentCombo = AttackStart.None;
        step = 0;


    }



IEnumerator ExecuteRightSweepCombo()
    {
        //yield return StartCoroutine(PerformRightSweep());
        yield return new WaitForSeconds(0.8f);

        if (Random.value < 0.5f) // Alegere aleatorie
        {
            //yield return StartCoroutine(PerformLeftSweep());

            yield return new WaitForSeconds(0.5f);

            //yield return StartCoroutine(Perform2SpinningSlashes());

            if(Random.value < 0.5f)
            {
                CurrentCombo = AttackStart.None;
                step = 0;
                yield break;
            }
            else
            {
                //yield return StartCoroutine(PerformDelayedLeftSweep());
            }
        }
        else
        {
            //yield return StartCoroutine(PerformDelayedLeftSweep());
        }

        CurrentCombo = AttackStart.None;
        step = 0;
    }

IEnumerator ExecuteDoubleSweepCombo()
    {
        //yield return StartCoroutine(PerformDoubleSweep());

        yield return new WaitForSeconds(0.8f);

        //yield return StartCoroutine(Perform2SpinningSlashes());

        if(Random.value < 0.5f)
        {
            //yield return StartCoroutine(PerformGroundSlam());

        }
        else
        {
            CurrentCombo = AttackStart.None;
            step = 0;
            yield break;
        }

        CurrentCombo = AttackStart.None;
        step = 0;
    }

IEnumerator ExecuteAoeDragCombo()
    {
        //yield return StartCoroutine(PerformAoeDrag());

        yield return new WaitForSeconds(1.5f);

        //yield return StartCoroutine(PerformExplosion());


        CurrentCombo = AttackStart.None;
        step = 0;

        
    }



    protected override void Start()
    {
        base.Start();

        initPool(nrMeteors);
    }

    private void initPool(int initialPoolSize)
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject meteor = Instantiate(meteorPrefab, transform.position, Quaternion.identity);

            meteor.SetActive(false);
            meteorPool.Enqueue(meteor);
        }
    }

    private GameObject GetMeteor()
    {
        if(meteorPool.Count == 0)
        {
            Debug.Log("Pool-ul a ramas fara meteoriti");
            initPool(1);
        }

        GameObject meteorToSpawn = meteorPool.Dequeue();

        meteorToSpawn.transform.position = spawnPoint.position;
        meteorToSpawn.transform.rotation = Quaternion.identity;
        meteorToSpawn.SetActive(true);

        return meteorToSpawn;
    }

    public void ReturnMeteorToPool(GameObject meteor)
    {
        meteor.SetActive(false);

        meteorPool.Enqueue(meteor);
    }
    private Vector3 GetTargetLaunchPosition(Vector3 targetPosition)
    {
        return new Vector3(targetPosition.x, meteorSpawnHeight, targetPosition.z);
    }


    protected override void AttackLogic()
    {

        float PlayerDistance = Vector3.Distance(transform.position, playerTarget.position);

        Vector3 lookDirection = playerTarget.position - transform.position;
        lookDirection.y = 0f;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        int choice = 0;

        if (CurrentCombo == AttackStart.None)
        {

            if (PlayerDistance > rangeAttackDistance)
            {
                choice = 0;

                switch (choice)
                {
                    case 0:
                        CurrentCombo = AttackStart.Meteors;
                        StartCoroutine(ExecuteMeteorsCombo());
                        break;
                    case 1:
                        CurrentCombo = AttackStart.Charge;
                        StartCoroutine(ExecuteChargeCombo());
                        break;
                    case 2:
                        CurrentCombo = AttackStart.AOEDrag;
                        StartCoroutine(ExecuteAoeDragCombo());
                        break;
                    case 3:
                        currentState = EnemyState.Chase;
                        if (agent != null) 
                            agent.isStopped = false;
                        return;
                        
                    default:
                        currentState = EnemyState.Chase;
                        if (agent != null) 
                            agent.isStopped = false; 
                        return;
                        
                }
            }
            else
            {
                choice = 2;

                switch (choice)
                {
                    case 0:
                        CurrentCombo = AttackStart.DoubleSweep;
                        StartCoroutine(ExecuteDoubleSweepCombo());
                        break;
                    case 1:
                        CurrentCombo = AttackStart.ExplodeSlash;
                        StartCoroutine(ExecuteExplodeSlashCombo());
                        break;
                    case 2:
                        CurrentCombo = AttackStart.Meteors;
                        StartCoroutine(ExecuteMeteorsCombo());
                        break;
                    case 3:
                        CurrentCombo = AttackStart.FrontFlip;
                        StartCoroutine(ExecuteFrontFlipCombo());
                        break;
                    case 4:
                        CurrentCombo = AttackStart.RightSweep;
                        StartCoroutine(ExecuteRightSweepCombo());
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
