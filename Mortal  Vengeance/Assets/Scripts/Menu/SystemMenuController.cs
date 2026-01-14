using UnityEngine;
using UnityEngine.SceneManagement;

// Controls the system menu and settings menu
public class SystemMenuController : MonoBehaviour
{
    enum MenuState  //menu states
    {
        Closed,
        System,
        Settings
    }

    MenuState currentState = MenuState.Closed;
    public GameObject systemMenu;
    public GameObject settingsMenu;
    public InventoryToggle inventoryToggle;
    public GameObject hotbarRoot;


    public static bool IsUIBlockingInput { get; private set; }

    public static void SetUIBlockingInput(bool value)
    {
        IsUIBlockingInput = value;
    }

    void Start()
    {
        Debug.Log("SystemMenuController START");
        systemMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape)){
            Debug.Log("Escape key pressed");
            HandleEscape();
            return;
        }

        if (SystemMenuController.IsUIBlockingInput)
            return;
    }

    //what happens when escape is pressed
    void HandleEscape()
    {
        switch (currentState)
        {
            case MenuState.Closed:
                OpenSystemMenu();
                break;

            case MenuState.System:
                Resume();
                break;

            case MenuState.Settings:
                CloseSettings();
                break;
        }
    }


    public void OpenSystemMenu()
    {
        if (inventoryToggle != null)
        {
            inventoryToggle.ForceCloseInventory();
        }

        if (hotbarRoot != null)
        {
            hotbarRoot.SetActive(false);
        }

        systemMenu.SetActive(true); //show main menu
        settingsMenu.SetActive(false); //hide settings menu

        Time.timeScale = 0f; //stop game time
        Cursor.visible = true; //show cursor
        Cursor.lockState = CursorLockMode.None;

        IsUIBlockingInput = true;//while menu is open, block game input
        currentState = MenuState.System; //set menu state to system
    }

    public void Resume()
    {
        systemMenu.SetActive(false);
        settingsMenu.SetActive(false);

        if (hotbarRoot != null)
        {
            hotbarRoot.SetActive(true);
        }

        Time.timeScale = 1f;  
        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Locked; 

        IsUIBlockingInput = false; 
        currentState = MenuState.Closed;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reload current scene
        IsUIBlockingInput = false; 
    }

    public void OpenSettings()
    {
        //check if we are in the system menu
        if (currentState != MenuState.System) 
            return;

        systemMenu.SetActive(false);
        settingsMenu.SetActive(true);

        IsUIBlockingInput = true;
        currentState = MenuState.Settings;
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        systemMenu.SetActive(true);

        IsUIBlockingInput = true;
        currentState = MenuState.System;
    }
}
