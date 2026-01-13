using UnityEngine;

public class PickupTextManager : MonoBehaviour
{
    public static PickupTextManager Instance;

    // singleton pattern
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        HidePrompt();
    }

    public void ShowPrompt()
    {
        gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        gameObject.SetActive(false);
    }
}