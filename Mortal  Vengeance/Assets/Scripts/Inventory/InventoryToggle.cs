using UnityEngine;
using UnityEngine.UI;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;
    public bool isOpen = false;

    [Header("Hotbar Reparenting")]
    public RectTransform hotbarPanel;
    public Transform hotbarHUDParent;
    public Transform hotbarInventoryParent;

    //to save hotbar position when reparenting
    private Vector2 savedAnchoredPosition;
    private Vector2 savedAnchorMin;
    private Vector2 savedAnchorMax;
    private Vector2 savedPivot;

    [Header("Layout")]
    public LayoutGroup hotbarLayoutGroup;


    //at the beginning of the game, the inventory is closed and the cursor is locked and invisible
    void Start()
    {
        inventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        savedAnchoredPosition = hotbarPanel.anchoredPosition;
        savedAnchorMin = hotbarPanel.anchorMin;
        savedAnchorMax = hotbarPanel.anchorMax;
        savedPivot = hotbarPanel.pivot;

    }

    
    void Update()
    {
        if (SystemMenuController.IsUIBlockingInput)//prevents opening the inventory if another menu is open
            return;

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
            //disable hotbar layout group to allow manual positioning
            hotbarLayoutGroup.enabled = false;

            //move the hotbar to the inventory panel
            hotbarPanel.SetParent(hotbarInventoryParent, false);

            //when the invenotiry is open, the cursor is not moving with the camera anymore and is visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;   //the game is paused when the inventory is open
        }
        else
        {
            //move the hotbar back to the HUD
            hotbarPanel.SetParent(hotbarHUDParent, false);

            //restore hotbar position
            hotbarPanel.anchorMin = savedAnchorMin;
            hotbarPanel.anchorMax = savedAnchorMax;
            hotbarPanel.pivot = savedPivot;
            hotbarPanel.anchoredPosition = savedAnchoredPosition;

            //re-enable hotbar layout group
            hotbarLayoutGroup.enabled = true;

            Cursor .lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }


    public void ForceCloseInventory()
    {
        if (!isOpen)
            return;

        isOpen = false;
        inventoryPanel.SetActive(false);

        // move hotbar back to HUD
        hotbarPanel.SetParent(hotbarHUDParent, false);

        hotbarPanel.anchorMin = savedAnchorMin;
        hotbarPanel.anchorMax = savedAnchorMax;
        hotbarPanel.pivot = savedPivot;
        hotbarPanel.anchoredPosition = savedAnchoredPosition;

        hotbarLayoutGroup.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

}
