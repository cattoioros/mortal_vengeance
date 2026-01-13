using UnityEngine;
using TMPro;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;

    private bool playerInRange;
    private bool pickedUp;

    void Update()
    {
        if (!playerInRange || pickedUp) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        InventoryUIManager inventory = FindObjectOfType<InventoryUIManager>(true);
        if (inventory == null) return;

        inventory.AddItem(item);

        pickedUp = true;

        if (PickupTextManager.Instance != null)
            PickupTextManager.Instance.HidePrompt();

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (PickupTextManager.Instance != null)
                PickupTextManager.Instance.ShowPrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (PickupTextManager.Instance != null)
                PickupTextManager.Instance.HidePrompt();
        }
    }

  
}
