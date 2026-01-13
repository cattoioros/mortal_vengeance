using System.Collections;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private PromisedBoss myBoss;

    private float dmgRadius;
    private Vector3 impactPosition;
    private int damageAmount;
    private float speed = 15f;

    private Rigidbody rb;


    //Layer for the meteors to hit/stop
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void SetMeteor(Vector3 targetPos, float delay, int damage, float radius, PromisedBoss boss)
    {
        //set the spawner and other parameters
        myBoss = boss;
        impactPosition = targetPos;
        damageAmount = damage;
        dmgRadius = radius;

        
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;

        StartCoroutine(LaunchAfterDelay(delay));
    }

    private IEnumerator LaunchAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        rb.isKinematic = false;

        //hit direction
        Vector3 direction = (impactPosition - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        //ignore boss or meteor collision
        if (other.CompareTag("Boss") || other.CompareTag("Meteor"))
            return;

        //ground hit
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            
            Collider[] hitObjects = Physics.OverlapSphere(transform.position, dmgRadius);

            //testing if the player was in the range
            foreach (Collider hit in hitObjects)
            {
                if (hit.TryGetComponent<PlayerStatsManager>(out var playerHealth))
                {
                    playerHealth.TakeDamage(damageAmount);
                    Debug.Log("Meteor: L-am lovit pe jucator!");
                }
            }

            myBoss?.ReturnMeteorToPool(gameObject);
        }
    }
}
