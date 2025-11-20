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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
    }


    public void SetMeteor(Vector3 targetPos, float delay, int damage, float radius, PromisedBoss boss)
    {
        myBoss = boss;
        impactPosition = targetPos;
        damageAmount = damage;
        dmgRadius = radius;

        if (rb != null)
            rb.isKinematic = true;

        StartCoroutine(LaunchAfterDelay(delay));
    }




    private IEnumerator LaunchAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (rb != null) rb.isKinematic = false;

        Vector3 direction = (impactPosition - transform.position).normalized;
        rb.linearVelocity = direction * speed;

    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, dmgRadius);

        foreach (Collider hit in hitObjects)
        {
            if (hit.TryGetComponent<PlayerHealthManager>(out var playerHealth))
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }

        if (myBoss != null)
            myBoss.ReturnMeteorToPool(gameObject);

    }
}


