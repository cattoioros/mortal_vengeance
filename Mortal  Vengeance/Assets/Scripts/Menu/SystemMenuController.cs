using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemMenuController : MonoBehaviour
{
    enum MenuState  //definirea starilor meniului
    {
        Closed,
        System,
        Settings
    }

    MenuState currentState = MenuState.Closed;
    public GameObject systemMenu;
    public GameObject settingsMenu;

    public static bool IsUIBlockingInput { get; private set; }

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

    //ce se intampla cand apas ESC in functie de starea meniului
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
        systemMenu.SetActive(true); //afisez meniul principal
        settingsMenu.SetActive(false); //ascund meniul de setari

        Time.timeScale = 0f; //opresc jocul
        Cursor.visible = true; //afisez cursorul
        Cursor.lockState = CursorLockMode.None;

        IsUIBlockingInput = true;//cat timp e meniul pornit nu se intampla nimic daca apas pe alte taste
        currentState = MenuState.System; //setez starea ca fiind meniul principal deschis
    }

    public void Resume()
    {
        systemMenu.SetActive(false);//ascund meniul principal
        settingsMenu.SetActive(false);//ascund meniul de setari

        Time.timeScale = 1f; //repornesc jocul 
        Cursor.visible = false; //ascund cursorul
        Cursor.lockState = CursorLockMode.Locked; 

        IsUIBlockingInput = false; //jocul poate primi input din nou
        currentState = MenuState.Closed;//setez starea meniului ca fiind inchis
    }

    public void Restart()
    {
        Time.timeScale = 1f; //repornesc jocul
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reincarc scena curenta
        IsUIBlockingInput = false; 
    }

    public void OpenSettings()
    {
        //verific daca sunt in meniul principal inainte de a deschide setarile
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
