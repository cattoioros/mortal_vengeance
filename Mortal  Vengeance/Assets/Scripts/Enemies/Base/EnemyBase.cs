using UnityEngine;
using UnityEngine.AI;


//Starile inamicilor
public enum EnemyState { Idle, Chase, Attack, Dead}

public class EnemyBase : MonoBehaviour
{
    //Caracteristici de baza ale tuturor inamicilor
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

        //Preluarea agentului pentru miscare
        agent = GetComponent<NavMeshAgent>();
        if(agent != null)
        {
            agent.speed = movementSpeed;
            agent.isStopped = true;
        }

        if(GameManager.instance != null)
        {
            playerTarget = GameManager.instance.PlayerTransform;

        }

        if (playerTarget == null)
        {
            Debug.LogError(gameObject.name + " nu a putut gasi jucătorul prin GameManager.");
        }

     

    }

    //Comportamentul inamicilor cat timp jucatorul nu este in raza
    protected virtual void UpdateIdle()
    {
        if (agent != null)
            agent.isStopped = true;

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
    }
    //Logica starii de atac
    protected virtual void UpdateAttack()
    {
        //Inamicul se roteste spre jucator
        Vector3 lookDirection = playerTarget.position - transform.position;

        lookDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

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

        if (currentHealth <= 0)
        {
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

        Debug.Log(gameObject.name + "a murit");

        Destroy(gameObject,3f);

       

    }

   

    // Update is called once per frame
    protected virtual void Update()
    {

        if (playerTarget == null || currentState == EnemyState.Dead)
            return;

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
