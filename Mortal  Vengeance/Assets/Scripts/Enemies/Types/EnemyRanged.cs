using UnityEngine;

public class EnemyRange : EnemyBase
{
    private float lastAttackTime;
    [Header("Ranged Specific")]
    [SerializeField] private GameObject arrow; 
    [SerializeField] private Transform firingPoint;


    protected override void UpdateChase()
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
    


    protected override void AttackLogic()
    {
        Debug.Log("Atac ranged");
    }
}
