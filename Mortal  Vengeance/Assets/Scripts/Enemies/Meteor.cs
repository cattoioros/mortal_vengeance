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


    //Layer pentru hit
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void SetMeteor(Vector3 targetPos, float delay, int damage, float radius, PromisedBoss boss)
    {
        //setare spawner, pozitie, dmg si raza de dmg
        myBoss = boss;
        impactPosition = targetPos;
        damageAmount = damage;
        dmgRadius = radius;

        //dezactivarea fizicii
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;

        StartCoroutine(LaunchAfterDelay(delay));
    }

    private IEnumerator LaunchAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        rb.isKinematic = false;

        //directia catre zona de impact
        Vector3 direction = (impactPosition - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        //ignora coliziunile cu boss sau alti meteoriti
        if (other.CompareTag("Boss") || other.CompareTag("Meteor"))
            return;

        //verificam daca loveste pamantul
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            //luam raza de coliziuni si aplicam dmg-ul in cazul jucatorului
            Collider[] hitObjects = Physics.OverlapSphere(transform.position, dmgRadius);

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
