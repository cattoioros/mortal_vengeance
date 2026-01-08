using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;
    public float pickupRadius = 2f;

    private Transform player;
    private bool pickedUp;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (pickedUp || player == null) return;

        if (Vector3.Distance(transform.position, player.position) <= pickupRadius)
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        if (pickedUp) return;

        InventoryUIManager inventory = FindObjectOfType<InventoryUIManager>(true);
        if (inventory == null) return;

        inventory.AddItem(item); 

        pickedUp = true;
        Destroy(gameObject);
    }
}
