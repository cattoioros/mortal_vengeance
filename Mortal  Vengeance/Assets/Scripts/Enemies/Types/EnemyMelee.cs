using UnityEngine;

public class EnemyMelee : EnemyBase
{
    private float lastAttackTime;


    protected override void AttackLogic()
    {
        Debug.Log("Atac Melee");
    }


}
