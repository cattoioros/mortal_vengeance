using UnityEngine;
using UnityEngine.AI;


public enum EnemyState { Idle, Chase, Attack, Dead}

public class EnemyBase : MonoBehaviour
{

    [Header("Stats & Ranges")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float chaseRange;
    [SerializeField] protected float attackRange;
    [SerializeField] protected int baseDmg;
    [SerializeField] protected float attackCooldown;

    protected int currentHealth; 
    protected EnemyState currentState = EnemyState.Idle;
    protected NavMeshAgent agent; 
    protected Transform playerTarget; 

    protected virtual void Start()
    {

        currentHealth = maxHealth;
        currentState = EnemyState.Idle;

        agent = GetComponent<NavMeshAgent>();
        if(agent != null)
        {
            agent.speed = movementSpeed;
            agent.isStopped = true;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player nu a fost gasit.");
        }
    }

    protected virtual void UpdateIdle()
    {
        if (agent != null)
            agent.isStopped = true;

        float distancePlayer = Vector3.Distance(transform.position, playerTarget.position);

        if(distancePlayer <= chaseRange)
        {
            currentState = EnemyState.Chase;
            if(agent != null)
                agent.isStopped = false;
        }
    }


    protected virtual void UpdateChase()
    {
        if(agent!=null)
        {
            agent.SetDestination(playerTarget.position);
        }

        float distancePlayer = Vector3.Distance(transform.position, playerTarget.position);

        if(distancePlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            if(agent != null )
                agent.isStopped = true;
            return;
        }

        if(distancePlayer > chaseRange)
            currentState = EnemyState.Idle;
        

    }

    protected virtual void UpdateAttack()
    {
        Vector3 lookDirection = playerTarget.position - transform.position;

        lookDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        float distancePlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distancePlayer > attackRange)
        {
            currentState = EnemyState.Chase;
            if (agent != null) agent.isStopped = false;
            return;
        }

        AttackLogic();

    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        currentState = EnemyState.Dead;
        if(agent != null)
        {
            agent.enabled = false;
        }

        Debug.Log(gameObject.name + "a murit");

        Destroy(gameObject,3f);

       

    }

   

    // Update is called once per frame
    protected virtual void Update()
    {

        if (playerTarget == null || currentState == EnemyState.Dead)
            return;

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

    protected virtual void AttackLogic()
    {

    }
}
