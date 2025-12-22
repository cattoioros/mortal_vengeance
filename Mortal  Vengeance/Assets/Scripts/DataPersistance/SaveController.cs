using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public PlayerHealthManager playerHealthManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SaveGame()
    {
        PlayerData data = playerHealthManager.GetPlayerData();
        SaveModel.Save(data);
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        PlayerData data = SaveModel.Load();
        if (data == null) return;

        playerHealthManager.ApplyPlayerData(data);
        Debug.Log("Game Loaded!");
    }
}
