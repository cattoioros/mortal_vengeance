using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PromisedBoss : EnemyBase
{
    [Header("Boss Specific")]
    [SerializeField] private float rangeAttackDistance;
    [SerializeField] private float chaseDuration = 2f;

    [Header("Meteors")]
    [SerializeField] private float meteorSpawnHeight = 20f;
    [SerializeField] private float meteorDelayTime = 3f;
    [SerializeField] private float meteorAoERadius = 3.5f;
    [SerializeField] private int nrMeteors = 5;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private int dmgFall;

    [Header("Visual Indicators")]
    [SerializeField] private GameObject groundDmgIndicator;
    [SerializeField] private GameObject sphereDmgIndicator;
    [SerializeField] private GameObject explosionIndicator;
    [SerializeField] private GameObject descentExplosionFx;

    [Header("Audio")]
    [SerializeField] private BossAudioController audioController;

    [Header("Light")]
    [SerializeField] private BossLightHalfHealth light;

    [Header("Weapon")]
    [SerializeField] private WeaponHitbox weaponHitbox;

    private bool is50Used = false;
    private Queue<GameObject> meteorPool = new Queue<GameObject>();

    private List<GameObject> activeIndicators = new List<GameObject>();

   //Init
    protected override void Start()
    {
        base.Start();
        InitMeteorPool(nrMeteors);
        isAttacking = false;
    }

    private void InitMeteorPool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject meteor = Instantiate(meteorPrefab, transform.position, Quaternion.identity);
            meteor.SetActive(false);
            meteorPool.Enqueue(meteor);
        }
    }

    private GameObject GetMeteor()
    {
        if (meteorPool.Count == 0) InitMeteorPool(1);
        GameObject meteor = meteorPool.Dequeue();
        meteor.transform.position = spawnPoint.position;
        meteor.SetActive(true);
        return meteor;
    }

    public void ReturnMeteorToPool(GameObject meteor)
    {
        meteor.SetActive(false);
        meteorPool.Enqueue(meteor);
    }

    // Animation Events
    public void ActivateWeaponHitbox()
    {
        weaponHitbox.damage = baseDmg;
        weaponHitbox.ActivateHitbox();
    }

    public void DeactivateWeaponHitbox() => weaponHitbox.DeactivateHitbox();
    public void PlaySwingSound() => audioController?.playSwordSwing();

    public void ApplyGroundSlamDmg() => StartCoroutine(GroundSlamSequence());

    public void OnSpinComplete()
    {
        if (Random.value < 0.5f)
            animator?.SetBool("GoGroundSlam", true);
        else
            isAttacking = false;
    }

    public void OnAttackComplete() => isAttacking = false;

    //movement for different attacks
    private IEnumerator MoveToPosition(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }


    //Apply dmg
    private void ApplySphereDamage(Vector3 center, float radius, int damage)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<PlayerStatsManager>(out var player))
            {
                player.TakeDamage(damage);
            }
        }
    }

    private void ApplyBoxDamage(Vector3 center, Vector3 extents, int damage)
    {
        Collider[] hits = Physics.OverlapBox(center, extents, transform.rotation);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<PlayerStatsManager>(out var player))
            {
                player.TakeDamage(damage);
            }
        }
    }

    // Attack sequences
    private IEnumerator GroundStompSequence()
    {
        float radius = 1.5f;
        GameObject warning = Instantiate(sphereDmgIndicator, agent.transform);
        activeIndicators.Add(warning);

        warning.transform.localPosition = new Vector3(0, 1f, 0);
        warning.transform.localScale = Vector3.one * (radius * 2f);

        yield return new WaitForSeconds(1f);

        Destroy(warning);
        ApplySphereDamage(transform.position, radius, baseDmg / 3);
    }

    private IEnumerator GroundSlamSequence()
    {
        // Sword swing damage
        Vector3 swordPos = transform.position + transform.forward * 1.5f;
        ApplyBoxDamage(swordPos, new Vector3(1f, 1f, 2f), baseDmg);

        // Delayed shockwave with warning
        Vector3 shockPos = transform.position + transform.forward * 4f;
        GameObject warning = Instantiate(groundDmgIndicator, shockPos, transform.rotation);
        activeIndicators.Add(warning);
        warning.transform.localScale = new Vector3(0.4f, 0.0001f, 0.65f);

        yield return new WaitForSeconds(1.5f);

        audioController?.GroundExplodeCue();
        ApplyBoxDamage(shockPos, new Vector3(3f, 2f, 4f), baseDmg);
        Destroy(warning);
    }

    private IEnumerator MeteorAttackSequence()
    {
        yield return GroundStompSequence();

        agent.updatePosition = false;

        // Rise
        Vector3 risePos = transform.position + Vector3.up * meteorSpawnHeight;
        yield return MoveToPosition(risePos, 8f);

        // Spawn meteors
        Vector3 targetPos = playerTarget.position;
        for (int i = 0; i < nrMeteors; i++)
        {
            float angle = i * Mathf.PI * 2f / nrMeteors;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 2.5f;

            GameObject meteor = GetMeteor();
            meteor.transform.position = transform.position + offset;

            if (meteor.TryGetComponent<Meteor>(out var script))
            {
                script.SetMeteor(targetPos, meteorDelayTime, baseDmg, meteorAoERadius, this);
            }
        }

        yield return new WaitForSeconds(meteorDelayTime - 1f);
        animator?.SetTrigger("TriggerMeteors");
        yield return new WaitForSeconds(0.5f);
        audioController?.playMeteorsCue();
        yield return new WaitForSeconds(2.5f);

        // Descend
        Vector3 descendPos = new Vector3(transform.position.x, transform.position.y - meteorSpawnHeight, transform.position.z);
        yield return MoveToPosition(descendPos, 20f);

        yield return new WaitForSeconds(2f);
        agent.updatePosition = true;
        isAttacking = false;
    }

    private IEnumerator ChargeAttackSequence()
    {
        agent.isStopped = true;
        agent.updatePosition = false;

        animator?.SetTrigger("TriggerCharge");

        // Jump and charge
        Vector3 jumpPos = transform.position + Vector3.up * 2f;
        Vector3 targetPos = new Vector3(playerTarget.position.x, jumpPos.y, playerTarget.position.z);
        yield return MoveToPosition(targetPos, 5f);

        //Plunge
        Vector3 plungePos = new Vector3(transform.position.x - 2f, transform.position.y - 2f, transform.position.z - 2f);
        yield return MoveToPosition(plungePos, 7f);

        GameObject warning = Instantiate(sphereDmgIndicator, transform.position, Quaternion.identity);
        activeIndicators.Add(warning);
        warning.transform.localScale = Vector3.one * 7f;

        yield return new WaitForSeconds(0.5f);

        Destroy(warning);
        GameObject explosion = Instantiate(explosionIndicator, transform.position, Quaternion.identity);
        activeIndicators.Add(explosion);
        explosion.transform.localScale = Vector3.one * 7f;
        audioController?.playExplosionCue();

        ApplySphereDamage(transform.position, 5f, baseDmg/2);
        Destroy(explosion, 2f);

   

        yield return new WaitForSeconds(2f);

        agent.isStopped = false;
        agent.updatePosition = true;
        isAttacking = false;
    }

    private IEnumerator ExplodeSlashSequence()
    {
        //Thrust
        yield return new WaitForSeconds(0.2f);
        animator?.SetTrigger("TriggerKick");

        Vector3 thrustPos = transform.position + transform.forward * 1.2f;
        yield return new WaitForSeconds(0.8f);
        ApplySphereDamage(thrustPos, 3f, baseDmg/2);

        // Two delayed explosions
        for (int i = 0; i < 2; i++)
        {
            GameObject warning = Instantiate(sphereDmgIndicator, thrustPos, Quaternion.identity);
            activeIndicators.Add(warning);
            warning.transform.localScale = Vector3.one * 5f;

            yield return new WaitForSeconds(1f);

            Destroy(warning);
            GameObject explosion = Instantiate(explosionIndicator, thrustPos, Quaternion.identity);
            activeIndicators.Add(explosion);
            explosion.transform.localScale = Vector3.one * 5f;
            audioController?.playExplosionCue();

            ApplySphereDamage(thrustPos, 4f, baseDmg / 2);
            Destroy(explosion, 2f);
        }

        yield return new WaitForSeconds(3f);
        isAttacking = false;
    }

    private IEnumerator AsteroidDescentSequence()
    {
        agent.isStopped = true;
        agent.updatePosition = false;

        // Teleport up
        transform.position = transform.position + Vector3.up * 100f;

        yield return new WaitForSeconds(5f);

        // Descend
        Vector3 targetPos = playerTarget.position;
        light?.StartDescent();
        audioController?.playDescentSound();

        yield return MoveToPosition(targetPos, 40f);

        light?.StopDescent();
        audioController?.playDescentCrash();

        // Explosion
        GameObject explosion = Instantiate(descentExplosionFx, transform);
        activeIndicators.Add(explosion);
        explosion.transform.localPosition = Vector3.zero;
        explosion.transform.localScale = Vector3.one * 30f;
        Destroy(explosion, 3f);

        ApplySphereDamage(targetPos, 15f, dmgFall);

        agent.isStopped = false;
        agent.updatePosition = true;
        agent.enabled = true;
        isAttacking = false;
    }

    private IEnumerator DoubleSweepSequence()
    {
        animator?.SetBool("GoGroundSlam", false);
        animator?.SetTrigger("TriggerSlashLR");
        yield return new WaitForSeconds(10f);
        isAttacking = false;
    }

    private IEnumerator RightSweepSequence()
    {
        animator?.SetInteger("DelayInt1", 0);
        animator?.SetInteger("DelayInt2", 0);
        animator?.SetTrigger("TriggerSlashRL");


        if (Random.value < 0.5f)
        {
            
            yield return new WaitForSeconds(0.5f);
            animator?.SetInteger("DelayInt1", 2);
            if (Random.value > 0.5f)
            {
                yield return new WaitForSeconds(2f);
                animator?.SetInteger("DelayInt2", 1);
            }
            else
            {
                animator?.SetInteger("DelayInt2", 2);
            }
        }
        else
        {
            yield return new WaitForSeconds(2f);
            animator?.SetInteger("DelayInt1", 1);
        }

        yield return new WaitForSeconds(3f);

        isAttacking = false;
    }

    private IEnumerator AoeDragSequence()
    {
        agent.isStopped = true;

        audioController?.playTrackSound();

        // Pull player
        Vector3 target = transform.position + transform.forward * 5f;
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Collider[] hits = Physics.OverlapSphere(transform.position, 20f);

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player") && hit.TryGetComponent<Transform>(out var tr))
                {
                    Debug.Log("Tras" + Time.time);
                    Vector3 dir = (transform.position - tr.position).normalized;
                    tr.position = Vector3.MoveTowards(tr.position, target, 10f * Time.deltaTime);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        animator?.SetTrigger("TriggerGroundSlam");
        yield return new WaitForSeconds(8f);

        agent.isStopped = false;
        isAttacking = false;
    }

    
    protected override void AttackLogic()
    {
      
        if (isAttacking) return;

        StopAllCoroutines();

        // Half health attack
        if ((!is50Used && currentHealth <= maxHealth / 2))
        {
            is50Used = true;
            isAttacking = true;
            StartCoroutine(AsteroidDescentSequence());
            return;
        }

        
        float distance = Vector3.Distance(transform.position, playerTarget.position);
        isAttacking = true;


       
        if (distance > rangeAttackDistance)
        {
            // Ranged attack
            int choice = Random.Range(0,3);
            
            switch (choice)
            {
                case 0:
                    {
                        StartCoroutine(MeteorAttackSequence()); 
                        Debug.Log("Meteors" + Time.time); 
                        break; 
                    }
                case 1:
                    {
                        StartCoroutine(ChargeAttackSequence());
                        Debug.Log("Charge" + Time.time);
                        break;
                    }
                case 2:
                    {
                        StartCoroutine(AoeDragSequence());
                        Debug.Log("AoeDrag" + Time.time);
                        break;
                    }
                        default:
                    agent.isStopped = false;
                    isAttacking = false;
                    break;
            }
        }
        else
        {
            // Melee attack
            int choice = Random.Range(0,3);
            switch (choice)
            {
                case 0:
                    {
                        StartCoroutine(DoubleSweepSequence());
                        Debug.Log("DoubleSweep" + Time.time);
                        break;
                    }
                case 1:
                    {
                        StartCoroutine(ExplodeSlashSequence());
                        Debug.Log("ExplodeSlash" + Time.time);
                        break;
                    }
                case 2:
                    {
                        StartCoroutine(RightSweepSequence());
                        Debug.Log("RightSweep" + Time.time);
                        break;
                    }
                default:
                    agent.isStopped = false;
                    isAttacking = false;
                    break;
            }
        }
    }

    protected override void UpdateChase()
    {
        base.UpdateChase();
        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    protected override void Update()
    {

        // Face player
        Vector3 lookDir = playerTarget.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        base.Update();
    }

    protected override void Die()
    {
        base.Die();

        foreach (GameObject indicator in activeIndicators)
        {
            if (indicator != null)
            {
                Destroy(indicator);
            }
        }

        if (audioController != null)
        {
            audioController.playDeathSound();
        }
    }
}