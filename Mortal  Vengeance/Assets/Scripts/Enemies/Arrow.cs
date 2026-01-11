using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    private EnemyRanged myArcher;
    private float launchTime;

    public void setArcher(EnemyRanged archer)
    {
        myArcher = archer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerStatsManager>(out var playerHealth))
        {
            playerHealth.TakeDamage(myArcher.arrowDmg);
            Debug.Log("Am lovit jucatorul");
        }

        Debug.Log("Lovit");

        myArcher.ReturnArrowToPool(gameObject);
    }

    public void OnEnable()
    {
        launchTime = Time.time;
    }

    public void Update()
    {
        if (Time.time - launchTime > 5)
        {
            myArcher.ReturnArrowToPool(gameObject);
        }

        
    }

}
