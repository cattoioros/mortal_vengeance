using System;
using UnityEngine;
using UnityEngine.AI;


//Starile inamicilor
public enum EnemyState { Idle, Chase, Attack, Dead}



public class EnemyBase : MonoBehaviour, Interfaces.IsDamageable
{
    //Caracteristici de baza ale tuturor inamicilor
    [Header("Stats & Ranges")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float chaseRange;
    [SerializeField] protected float attackRange;
    [SerializeField] protected int baseDmg;
    [SerializeField] protected float attackCooldown;

    [Header("UI")]
    [SerializeField] protected EnemyHealthUI healthBar;

    protected int currentHealth; 
    protected EnemyState currentState = EnemyState.Idle;
    protected NavMeshAgent agent; 
    protected Transform playerTarget;
    protected Animator animator;
    protected bool isAttacking = false;
    protected bool isDead = false;


    protected virtual void Start()
    {

        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<EnemyHealthUI>();
        }


        currentHealth = maxHealth;
        if (healthBar != null )
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        currentState = EnemyState.Idle;

        //Preluarea agentului pentru miscare
        agent = GetComponent<NavMeshAgent>();
        if(agent != null)
        {
            agent.speed = movementSpeed;
            agent.isStopped = true;

            agent.stoppingDistance = attackRange;
            agent.acceleration = agent.speed * 2;
            agent.angularSpeed = 360f;
        }

        if(GameManager.instance != null)
        {
            playerTarget = GameManager.instance.PlayerTransform;

        }
        else
        {
            Debug.Log("Nu avem instanta");
        }

        if (playerTarget == null)
        {
            Debug.LogError(gameObject.name + " nu a putut gasi jucătorul prin GameManager.");
        }


        animator = GetComponentInChildren<Animator>(); // <- asta lipsea

        if (animator == null)
            Debug.LogError(name + " nu are animator");



    }

    //Comportamentul inamicilor cat timp jucatorul nu este in raza
    protected virtual void UpdateIdle()
    {
        if (agent != null)
            agent.isStopped = true;

        if (animator != null)
            animator.SetFloat("Speed", 0);

        float distancePlayer = Vector3.Distance(transform.position, playerTarget.position);

        //Schimbarea starii in cazul in care jucatorul intra in raza inamicului
        if(distancePlayer <= chaseRange)
        {
            currentState = EnemyState.Chase;
            if(agent != null)
                agent.isStopped = false;
        }
    }

    //Logica starii de urmarire
    protected virtual void UpdateChase()
    {
        if (agent != null)
            agent.isStopped = false;
        float distancePlayer = Vector3.Distance(transform.position, playerTarget.position);


        if (distancePlayer > chaseRange)
        {
            currentState = EnemyState.Idle;
            if (agent != null)
                agent.isStopped = true;
            return;
        }

        if (distancePlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            if (agent != null)
                agent.isStopped = true;
            return;
        }

        if (agent != null)
            agent.SetDestination(playerTarget.position);

        if (agent != null && animator != null)
        {
            // currentSpeed este viteza pe care o folosește NavMeshAgent-ul
            float currentSpeed = agent.velocity.magnitude;

            // Setează parametrul "Speed" în Animator
            animator.SetFloat("Speed", currentSpeed);
        }
    }
    //Logica starii de atac
    protected virtual void UpdateAttack()
    {

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
            animator.SetFloat("Speed", 0f);

        }

        //Inamicul se roteste spre jucator
        Vector3 lookDirection = playerTarget.position - transform.position;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        //Verificam daca jucatorul iese din raza de atac
        float distancePlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distancePlayer > attackRange)
        {
            currentState = EnemyState.Chase;
            if (agent != null) agent.isStopped = false;
            return;
        }
        //Efectuare atacului
        AttackLogic();

    }

    //Functie pentru ranirea inamicilor
    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Am luat damage"+ amount);

        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0 && !isDead )
        {
            isDead = true;
            Die();
        }
    }

    //Moartea inamicilor
    protected virtual void Die()
    {
        currentState = EnemyState.Dead;
        if(agent != null)
        {
            agent.enabled = false;
        }

        if(animator!= null)
        {
            animator.SetTrigger("TriggerDeath");
        }

        Debug.Log(gameObject.name + "a murit");

        Destroy(gameObject, 5f);
       

    }

   

    // Update is called once per frame
    protected virtual void Update()
    {

       


       
        //Debug.Log(currentState);
        if (playerTarget == null || currentState == EnemyState.Dead)
            return;

        if (isAttacking) return;

        //Schimbarea intre stari
        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
        }

        
    }

    //Logica de atac( Va fi implementata de fiecare casa depinzand de tipul atacului)
    protected virtual void AttackLogic()
    {

    }
}
