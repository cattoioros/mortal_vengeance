using KevinIglesias;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : EnemyBase
{
    private float lastAttackTime = 0;

    private Vector3 pozLoadSageataInitial;
    private Vector3 rotatieBrat1Initial;
    private Vector3 rotatieBrat2Initial;

    [Header("Ranged Specific")]
    [SerializeField] private GameObject arrowPrefab; 
    [SerializeField] private float speed = 50f;
    [SerializeField] private int initialPoolSize =3;
    [SerializeField] public int arrowDmg;



    private Queue<GameObject> arrowPool = new Queue<GameObject>();

    [Header("BOW")]
    public LineRenderer sfoara;

    public Transform bratArc1;
    public Transform bratArc2;

    public Transform varfArc1;
    public Transform varfArc2;
    public Transform punctLoadSageata;

    public Transform bowstringAnchorPoint;

    public AnimationCurve bowReleaseCurve;

    private Vector3 pozLoadSageata;
    private Vector3 rotatieBrat1;
    private Vector3 rotatieBrat2;

    [Header("ARROW")]
    public GameObject arrowInHand;

    [Header("Audio")]
    public RangedAudioController audioController;




    protected override void Start()
    {
        base.Start();

        initPool(initialPoolSize);

        pozLoadSageata = punctLoadSageata.localPosition;

        rotatieBrat1 = bratArc1.localEulerAngles;
        rotatieBrat2 = bratArc2.localEulerAngles;
        arrowInHand.SetActive(false);


        rotatieBrat1Initial = rotatieBrat1;
        rotatieBrat2Initial = rotatieBrat2;
        pozLoadSageataInitial = pozLoadSageata;
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

        Debug.Log("Luam sageata" + Time.time);

        //Scoatem sageata din pool
        GameObject arrowToSpawn = arrowPool.Dequeue();

        arrowToSpawn.transform.position = punctLoadSageata.position;
        arrowToSpawn.transform.rotation = Quaternion.identity;
        arrowToSpawn.SetActive(true);

        return arrowToSpawn;
    }


    public void LoadArrow()
    {
        Debug.Log("Incarcam sageata" + Time.time);
        arrowInHand.SetActive(true);
    }

    public void UnLoadArrow()
    {
        Debug.Log("Descarcam sageata" + Time.time);
        arrowInHand.SetActive(false);
    }





    public void ReturnArrowToPool(GameObject arrow)
    {
        arrow.SetActive(false);

        arrowPool.Enqueue(arrow);
    }

    private void initSfoara()
    {

        sfoara.useWorldSpace = true; 
        sfoara.positionCount = 3;
        sfoara.SetPosition(0, varfArc1.position);
        sfoara.SetPosition(1, punctLoadSageata.position);
        sfoara.SetPosition(2, varfArc2.position);
    }


    private void LateUpdate()
    {
        
        initSfoara();
    }


    private IEnumerator LoadBow(float delay, float duration)
    {
        Debug.Log("Pool count" + arrowPool.Count);
        yield return new WaitForSeconds(delay);

        Vector3 brat1RotatieLoad = rotatieBrat1 + new Vector3(0, 0, -15);
        Vector3 brat2RotatieLoad = rotatieBrat2 + new Vector3(0, 0, -15);

        punctLoadSageata.localPosition = pozLoadSageata;

        if (audioController != null)
        {
            audioController.playLoadBow();
        }

        float t = 0;
        while(t <1)
        {
            t += Time.deltaTime / duration;

            bratArc1.localEulerAngles = Vector3.Lerp(rotatieBrat1, brat1RotatieLoad, t);
            bratArc2.localEulerAngles = Vector3.Lerp(rotatieBrat2, brat2RotatieLoad, t);

            punctLoadSageata.position = Vector3.Lerp(punctLoadSageata.position, bowstringAnchorPoint.position, t);



            yield return null;
        }

        
    }


    public void ShootArrow()
    {
        Debug.Log("Incepem operatiunea de tragere" + Time.time);
        GameObject readyArrow = GetArrow();
        Vector3 PlayerDirection = playerTarget.transform.position - punctLoadSageata.position;
        readyArrow.transform.rotation = Quaternion.LookRotation(PlayerDirection);


        Rigidbody rb = readyArrow.GetComponent<Rigidbody>();

        rb.linearVelocity = PlayerDirection.normalized * speed;

        Arrow arrowScript = readyArrow.GetComponent<Arrow>();

        arrowScript.setArcher(this);

        if(audioController != null)
        {
            audioController.playShootBow();
        }

        StartCoroutine(ReleaseBow(0.2f));
    }

    private IEnumerator ReleaseBow(float duration)
    {
        Debug.Log("Release bow" + Time.time);
        float t = 0;
        Vector3 brat1Start = bratArc1.localEulerAngles;
        Vector3 brat2Start = bratArc2.localEulerAngles;
        Vector3 punctStart = punctLoadSageata.localPosition;

        while (t < 1)
        {
            t += Time.deltaTime / duration;

            bratArc1.localEulerAngles = Vector3.Lerp(brat1Start, rotatieBrat1Initial, t);
            bratArc2.localEulerAngles = Vector3.Lerp(brat2Start, rotatieBrat2Initial, t);
            punctLoadSageata.localPosition = Vector3.Lerp(punctStart, pozLoadSageataInitial, t);

            yield return null;
        }

        isAttacking = false;
        agent.isStopped = false;

    }


    protected override void AttackLogic()
    {
        agent.isStopped = true;
        if (Time.time > lastAttackTime + attackCooldown || lastAttackTime ==0)
        {
            if(animator !=null)
            {
                isAttacking = true;
                animator.SetTrigger("TriggerAttack");
                StartCoroutine(LoadBow(0.6f, 1f));
            }

            lastAttackTime = Time.time;


            




        }
    }

    protected override void Update()
    {
        base.Update();

       
        // Face player
        Vector3 lookDir = playerTarget.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

}
