using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;
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
    [SerializeField] private int dmgFall;


    [Header("Visual Indicators")]
    [SerializeField] private GameObject GroundDmgIndicator;
    [SerializeField] private GameObject SphereDmgIndicator;
    [SerializeField] private GameObject explosionIndicator;
    [SerializeField] private GameObject descentExplosionFx;


    [Header("Audio")]
    public BossAudioController audioController; 

    //flag atac dat o singura data la jumatate din viata
    private bool is50Used = false;

    [Header("Light")]
    public BossLightHalfHealth light;

    
    //pool pentru meteoriti
    private Queue<GameObject> meteorPool = new Queue<GameObject>();

    [Header("WeaponHitbox")]
    public WeaponHitbox weaponHitbox;

    public void ActivateWeaponHitbox()
    {
        weaponHitbox.damage = this.baseDmg;
        weaponHitbox.ActivateHitbox();
    }

    public void DeactivateWeaponHitbox()
    {
        weaponHitbox.DeactivateHitbox();
    }


    public void PlaySwingSound()
    {
        if (audioController != null)
        {
            audioController.playSwordSwing();
        }
    }

    //enum cu combo-urile
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
        HalfHealth,

    }

    //combo-ul curent al boss-ului
    private AttackStart CurrentCombo = AttackStart.None;

    //private Animator animator;
    
    
    //atac GroundStomp
    private IEnumerator PerformGroundStomp()
    {

        float dmgRadius = 1.5f; 
        

        Debug.Log("Boss: Incep Stomp-ul de la sol.");
        GameObject dmgSphereZone = Instantiate(SphereDmgIndicator, agent.transform);
        dmgSphereZone.transform.localPosition = new Vector3(0,1f,0);
        dmgSphereZone.transform.localScale = Vector3.one * (dmgRadius *2);


        yield return new WaitForSeconds(1f);

        Destroy(dmgSphereZone);

        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, dmgRadius); 
        foreach (var hit in hitPlayers)
        {
            if (hit.TryGetComponent<PlayerHealthManager>(out var playerHealth))
            {
                playerHealth.TakeDamage(baseDmg / 3);
                Debug.Log("Am nimerit cu GroundStomp");
            }
        }
        Debug.Log("Boss: Daune mici aplicate. Trec in aer.");


        yield return new WaitForSeconds(1.0f);
    }

    //miscare pana la un anumit punct
    IEnumerator PerformMove(Vector3 targetPos, float speed)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
    }

    //atac meteoriti
    IEnumerator PerformMeteors()
    {
        yield return PerformGroundStomp();

        if (agent != null)
        {
            agent.updatePosition = false;
        }



        //boss-ul se ridica
        Vector3 positionUp = new Vector3(transform.position.x, transform.position.y + meteorSpawnHeight, transform.position.z);

        yield return PerformMove(positionUp,8f);

        Debug.Log("Meteoriti spawning");

        Vector3 playerPos = playerTarget.position;
        

        //incepe spawn-ul de meteoriti
        for (int i = 0; i < nrMeteors; i++)
        {
            float angle = i * Mathf.PI * 2 / nrMeteors;
            Vector3 spawnOffset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 2.5f;

            GameObject meteor = GetMeteor();

            meteor.transform.position = transform.position + spawnOffset ;

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
            }


        }

        yield return new WaitForSeconds(meteorDelayTime - 1);
        
        if (animator!= null)
        {
            animator.SetTrigger("TriggerMeteors");
        }

        yield return new WaitForSeconds(0.5f);
        
        if(audioController != null)
        {
            audioController.playMeteorsCue();
        }

        yield return new WaitForSeconds(3f);


        //revine jos
        Vector3 positionDown = new Vector3(transform.position.x, transform.position.y - meteorSpawnHeight, transform.position.z);

        yield return PerformMove(positionDown, 12f);


        CurrentCombo = AttackStart.None;

        if (agent != null)
        {
            agent.updatePosition = true;
        }


    }


    //atac charge, boss-ul sare, se duce spre jucator si apoi coboara aplicand daune
    IEnumerator PerformCharge()
    {
        Vector3 jumpPosition = new Vector3(transform.position.x,transform.position.y + 2f, transform.position.z);

        Vector3 chargePosition = new Vector3(playerTarget.position.x, jumpPosition.y, playerTarget.position.z);

        yield return PerformMove(chargePosition, 5f);

        Vector3 plungePosition = new Vector3(transform.position.x,transform.position.y - 2f, transform.position.z);

        yield return PerformMove(plungePosition, 7f);

        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, 1.5f);

        foreach (var hit in hitPlayers)
        {
            if (hit.TryGetComponent<PlayerHealthManager>(out var playerHealth))
            {
                playerHealth.TakeDamage(baseDmg);
                Debug.Log("AM lovit");
            }
           
        }

    }

    //combo meteoriti
    IEnumerator ExecuteMeteorsCombo()
    {
        yield return StartCoroutine(PerformMeteors());

        CurrentCombo = AttackStart.None;
    }

    //charge combo
    IEnumerator ExecuteChargeCombo()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.updatePosition = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("TriggerCharge");
        }

        yield return StartCoroutine(PerformCharge());

        if (agent != null)
        {
            agent.isStopped = false;
            agent.updatePosition = true;
        }

        CurrentCombo = AttackStart.None;
    }



    void DoThrustDamage(float range, float radius)
    {
        Vector3 center = transform.position + transform.forward * range;

        Collider[] hits = Physics.OverlapSphere(center, radius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<PlayerHealthManager>(out var player))
            {
                player.TakeDamage(baseDmg);
                Debug.Log("Thrust: Player lovit");
            }
        }
    }


    IEnumerator PerformForwardThrustAttack()
    {
        yield return new WaitForSeconds(0.2f);

        if (animator != null)
        {
            animator.SetTrigger("TriggerKick");
        }

        DoThrustDamage(1.2f, 1f);

        yield return new WaitForSeconds(0.4f);
    }

    IEnumerator DelayedExplosionDamage(Vector3 center, float radius, float dmg)
    {
        Debug.Log("Incepem explozia");


        GameObject SphereZone = Instantiate(SphereDmgIndicator);
        SphereZone.transform.position = center;
        SphereZone.transform.localScale = Vector3.one * (2 * radius);
        SphereZone.transform.rotation = Quaternion.identity;


        yield return new WaitForSeconds(1f);
        Destroy(SphereZone);
        GameObject ExplosionZone = Instantiate(explosionIndicator);
        ExplosionZone.transform.position = center;
        ExplosionZone.transform.localScale = Vector3.one * (2 * radius);
        ExplosionZone.transform.rotation = Quaternion.identity;

        audioController.playExplosionCue();

        Collider[] hits = Physics.OverlapSphere(center, radius);

        
        Destroy(ExplosionZone, 2f);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<PlayerHealthManager>(out var player))
            {
                player.TakeDamage(dmg);
                Debug.Log("Explozie: player lovit");
            }
        }
    }


    IEnumerator PerformExplodeSlash()
    {
        yield return StartCoroutine(PerformForwardThrustAttack());

        Vector3 thrustCenter = transform.position + transform.forward * 1.2f;
        


        

        yield return StartCoroutine(DelayedExplosionDamage(thrustCenter, 2.5f, baseDmg * 2));

        yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine(DelayedExplosionDamage(thrustCenter, 2.5f, baseDmg * 2));
    }

    IEnumerator ExecuteExplodeSlashCombo()
    {
        yield return StartCoroutine(PerformExplodeSlash());


        CurrentCombo = AttackStart.None;
    }


    IEnumerator PerformAsteroidDescent()
    {
        Debug.Log("Incepem saritura");

        if (agent != null)
        {
            agent.isStopped = true;
            agent.updatePosition = false; 
                     
        }


        Vector3 AscendPosition = new Vector3(transform.position.x, transform.position.y + 100f, transform.position.z);

        transform.position = AscendPosition;

   

       
        Vector3 DescendPosition = playerTarget.position;

        yield return new WaitForSeconds(5f);

        Debug.Log("Incepem coborarea");

        if (light != null)
        {
            light.StartDescent();
        }

        if (audioController != null)
        {
            audioController.playDescentSound();
        }



        yield return StartCoroutine(PerformMove(DescendPosition,40f));
        if (light != null)
        {
            light.StopDescent();
        }

        if(audioController!=null)
        {
            audioController.playDescentCrash();
        }

        GameObject dangerZone = Instantiate(descentExplosionFx,transform);
        dangerZone.transform.localPosition = Vector3.zero;
        dangerZone.transform.localScale = Vector3.one * (15f * 2f);

        Destroy(dangerZone,3f);

        Collider[] hits = Physics.OverlapSphere(DescendPosition, 15f);

        foreach (Collider hit in hits)
        {
            if(hit.TryGetComponent<PlayerHealthManager>(out var playerHealth))
            {
                playerHealth.TakeDamage(dmgFall);
                Debug.Log("Descent Explosion hit player");
            }
        }

        if (agent != null)
        {
            agent.isStopped = false;
            agent.updatePosition = true;
            agent.enabled = true;
        }

        CurrentCombo = AttackStart.None;
    }


    IEnumerator ExecuteRightSweepCombo()
        {
            animator.SetBool("GoSlashLR", false);
            animator.SetBool("GoDelay", false);

            if (animator!= null)
            {
                animator.SetTrigger("TriggerSlashRL");
            }
            
            yield return new WaitForSeconds(0.8f);

            if (Random.value < 0.5f) // Alegere aleatorie
            {
                animator.SetBool("GoSlashLR", true);

                yield return new WaitForSeconds(0.5f);
                if(Random.value < 0.5f)
                {
                    CurrentCombo = AttackStart.None;
                    yield break;
                }
                else
                {
                    //Delay 2 secunde (sta cu sabia in aer inainte sa atace)
                    yield return new WaitForSeconds(2f);
                    animator.SetBool("GoDelay", true);
                }
            }
            else
            {
                //Delay 2 secunde (sta cu sabia in aer inainte sa atace)
                yield return new WaitForSeconds(2f);
                
            }

        CurrentCombo = AttackStart.None;
        }


    public void OnSpinComplete()
    {
        // 50/50 pentru GroundSlam
        if (Random.value < 0.5f)
        {
            if (animator != null)
            {
                Debug.Log("Spin complet - trecem la GroundSlam");
                animator.SetBool("GoGroundSlam", true);
            }
        }
        else
        {
            Debug.Log("Spin complet - trecem la Idle");
            CurrentCombo = AttackStart.None;
        }
    }


    IEnumerator PerformDoubleSweep()
    {


        //sweep in stanga apoi in dreapta
        //deoarece in animator nu am pus trigger la trecerea de la sweep LR -> RL, nu este nevoia de alt SetTrigger, se va face automat trecerea
        //
        if(animator!=null)
        {
            animator.SetTrigger("TriggerSlashLR");
        }

        

        yield return new WaitForSeconds(2.0f);


    }

    IEnumerator DmgGroundSlamSeq()
    {
        Debug.Log("Aplica daune Sabie + Shockwave.");


        Vector3 box = transform.position + transform.forward * 1.5f;
        Vector3 extend = new Vector3(1f, 1f, 2f);
        Collider[] hits = Physics.OverlapBox(box, extend, transform.rotation);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<PlayerHealthManager>(out var player))
            {
                player.TakeDamage(baseDmg);
                Debug.Log("Lovit cu sabia");
            }
        }


        Vector3 boxGround = transform.position + transform.forward * 4f;
        Vector3 extendGround = new Vector3(3f, 2f, 4f);
        GameObject dangerZone = Instantiate(GroundDmgIndicator, boxGround, transform.rotation);
        dangerZone.transform.localScale = new Vector3(0.4f, 0.0001f, 0.65f);

        yield return new WaitForSeconds(1.5f);

        audioController.GroundExplodeCue();

        Collider[] hitsGround = Physics.OverlapBox(boxGround, extendGround, transform.rotation);

        foreach (var hit in hitsGround)
        {
            if (hit.TryGetComponent<PlayerHealthManager>(out var player))
            {
                player.TakeDamage(baseDmg);
                Debug.Log("Lovit de shockwave");
            }
        }

        Destroy(dangerZone);
    }
    public void ApplyGroundSlamDmg()
    {
        StartCoroutine(DmgGroundSlamSeq());
    }

    IEnumerator ExecuteDoubleSweepCombo()
        {
            animator.SetBool("GoGroundSlam", false);

            yield return StartCoroutine(PerformDoubleSweep());

            yield return new WaitForSeconds(0.8f);

            CurrentCombo = AttackStart.None;

        }

    //jucatorul este tras catre boss
    IEnumerator PerformAoeDrag(float dragDuration, float AoeRadius, float dragStrenght)
    {
        Vector3 center = transform.position;

        float t = 0;

        yield return new WaitForSeconds(1f);


        while (t < dragDuration)
        {
            Debug.Log("Incepem Drag-ul");
            t += Time.deltaTime;


            Collider[] hits = Physics.OverlapSphere(center, AoeRadius);

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player")){
                    Debug.Log("A lovit player-ul");

                    Rigidbody rb = hit.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 dir = (center - rb.position).normalized;
                        rb.MovePosition(rb.position + dir * dragStrenght * Time.deltaTime);
                    }
                }
            }
        }

    }
    IEnumerator ExecuteAoeDragCombo()
        {

            Debug.Log("Incepem combo aoeDrag");

            if (agent != null)
                agent.isStopped = true;
        
            yield return StartCoroutine(PerformAoeDrag(1.5f,20f,4f));

            yield return new WaitForSeconds(1.5f);


            if(animator != null)
                {
                    animator.SetTrigger("TriggerGroundSlam");
                }

            yield return new WaitForSeconds(5.0f);

            CurrentCombo = AttackStart.None;

            animator.ResetTrigger("TriggerGroundSlam");

            if (agent != null)
                agent.isStopped = false;


    }



    protected override void Start()
    {
        base.Start();

        initPool(nrMeteors);

        animator = GetComponent<Animator>();
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

    protected override void UpdateChase()
    {
        base.UpdateChase();

        if(animator != null && agent != null)
        {
            float curSpeed = agent.velocity.magnitude;

            animator.SetFloat("Speed", curSpeed);
        }
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

        if (animator != null && agent != null)
        {
            float speed = agent.velocity.magnitude;

            animator.SetFloat("Speed", speed);
        }


        if (CurrentCombo == AttackStart.None)
        {

            if ((!is50Used && currentHealth <= maxHealth / 2))
            {
                CurrentCombo = AttackStart.HalfHealth;
                StartCoroutine(PerformAsteroidDescent());
                is50Used = true;
            }
            else if (PlayerDistance > rangeAttackDistance)
            {
                choice = Random.Range(0, 4);

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
                        break;
                        
                    default:
                        currentState = EnemyState.Chase;
                        if (agent != null) 
                            agent.isStopped = false; 
                        break;
                        
                }
            }
            else
            {
                choice = Random.Range(0,4);
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
