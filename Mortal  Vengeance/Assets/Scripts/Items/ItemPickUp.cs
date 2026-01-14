using UnityEngine;
using TMPro;

// system for picking up items in the game world
public class ItemPickup : MonoBehaviour
{
    public ItemData item;

    private bool playerInRange;
    private bool pickedUp;

    // Check for player input to pick up the item
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

    // Detect when the player enters the pickup range
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (PickupTextManager.Instance != null)
                PickupTextManager.Instance.ShowPrompt();
        }
    }

    // Detect when the player exits the pickup range
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
