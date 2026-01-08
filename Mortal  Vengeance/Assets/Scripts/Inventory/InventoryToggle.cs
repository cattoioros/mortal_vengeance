using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;
    public bool isOpen = false;

    //at the beginning of the game, the inventory is closed and the cursor is locked and invisible
    void Start()
    {
        inventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            ToggleInventory();
        }
    }

    //function that opens and closes the inventory panel
    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            //when the invenotiry is open, the cursor is not moving with the camera anymore and is visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;   //the game is paused when the inventory is open
        }
        else
        {
            Cursor .lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
}
