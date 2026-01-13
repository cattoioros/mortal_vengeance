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
       

        Debug.Log($"Lovit: {other.name}, Tag: {other.tag}");

        if (other.transform == myArcher.transform || other.transform.IsChildOf(myArcher.transform))
        {
            Debug.Log("Ignorat, my archer");
            return;
        }

        if(other.CompareTag("Enemy"))
        {
            Debug.Log("Am nimerit un inamic");
            return;
        }

        if (other.TryGetComponent<PlayerStatsManager>(out var playerHealth))
        {
            playerHealth.TakeDamage(myArcher.arrowDmg);
            Debug.Log("Am lovit jucatorul");
        }

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
            Debug.Log("Return, sageata merge infinit");
            myArcher.ReturnArrowToPool(gameObject);
        }
    }
}