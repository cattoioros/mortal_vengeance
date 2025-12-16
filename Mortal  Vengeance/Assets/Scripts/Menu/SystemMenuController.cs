using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemMenuController : MonoBehaviour
{
    public GameObject systemMenu;
    void Start()
    {
        Debug.Log("SystemMenuController START");
        systemMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            Debug.Log("Escape key pressed");
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        bool isOpen = !systemMenu.activeSelf;
        systemMenu.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;  //pentru valoarea 0 jocul se opreste(pauza) iar pentru 1 ruleaza normal
        Cursor.visible = isOpen;  //cursor pentru meniu vizibil sau invizibil
        Cursor.lockState=isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void Resume()
    {
        systemMenu.SetActive(false);//ascund meniul
        Time.timeScale = 1f; //repornesc jocul 
        Cursor.visible = false; //ascund cursorul
        Cursor.lockState = CursorLockMode.Locked; 
    }

    public void Restart()
    {
        Time.timeScale = 1f; //repornesc jocul
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reincarc scena curenta
    }
}
