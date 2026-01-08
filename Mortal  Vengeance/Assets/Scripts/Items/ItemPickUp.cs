using UnityEngine;
using TMPro;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;

    [Header("UI")]
    [SerializeField] private GameObject pickupPrompt; 

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
        HidePrompt();
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowPrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
        }
    }

    void ShowPrompt()
    {
        if (pickupPrompt != null)
            pickupPrompt.SetActive(true);
    }

    void HidePrompt()
    {
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }
}
