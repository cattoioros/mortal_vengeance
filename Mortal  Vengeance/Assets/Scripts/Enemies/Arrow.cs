using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    private EnemyRanged myArcher;

    public void setArcher(EnemyRanged archer)
    {
        myArcher = archer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerHealthManager>(out var playerHealth))
        {
            playerHealth.TakeDamage(myArcher.arrowDmg);
        }

        Debug.Log("Lovit");

        myArcher.ReturnArrowToPool(gameObject);
    }

}
