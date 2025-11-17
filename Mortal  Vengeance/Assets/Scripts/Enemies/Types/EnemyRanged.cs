using UnityEngine;

public class EnemyRanged : EnemyBase
{
    private float lastAttackTime;
    [Header("Ranged Specific")]
    [SerializeField] private GameObject arrow; 
    [SerializeField] private Transform firingPoint;


   
    


    protected override void AttackLogic()
    {
        Debug.Log("Atac ranged");
    }
}
