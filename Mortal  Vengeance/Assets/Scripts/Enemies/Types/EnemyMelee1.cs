using UnityEngine;

public class EnemyMelee1 : EnemyBase
{
    private float lastAttackTime;

    [Header("Audio")]
    public MeleeAudioController audioController;

    [Header("Weapon")]
    [SerializeField] private WeaponHitbox weaponHitbox;


    public void ActivateWeaponHitbox()
    {
        weaponHitbox.damage = baseDmg;
        weaponHitbox.ActivateHitbox();
    }

    public void DeactivateWeaponHitbox() => weaponHitbox.DeactivateHitbox();

    public void AttackStart()
    {
        isAttacking = true;
    }
    public void AttackEnd()
    {
        isAttacking = false;
    }

    public void AttackAudio()
    {
        if(audioController!=null)
        {
            audioController.playSwordAttack();
        }
    }


    protected override void AttackLogic()
    {

        if (isAttacking) return;


        if (Time.time > lastAttackTime + attackCooldown)
        {

            lastAttackTime = Time.time;
            isAttacking = true;
            if(animator!=null)
            {

                animator.SetTrigger("TriggerAttack");
            }

        }

        Debug.Log("Atac melee wide");

    }


}



