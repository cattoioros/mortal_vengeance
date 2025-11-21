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

    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void SetMeteor(Vector3 targetPos, float delay, int damage, float radius, PromisedBoss boss)
    {
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

        Vector3 direction = (impactPosition - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss") || other.CompareTag("Meteor"))
            return;

        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Collider[] hitObjects = Physics.OverlapSphere(transform.position, dmgRadius);

            foreach (Collider hit in hitObjects)
            {
                if (hit.TryGetComponent<PlayerHealthManager>(out var playerHealth))
                {
                    playerHealth.TakeDamage(damageAmount);
                    Debug.Log("Meteor: L-am lovit pe jucator!");
                }
            }

            myBoss?.ReturnMeteorToPool(gameObject);
        }
    }
}
