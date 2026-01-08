using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + "/savefile.json";
        File.WriteAllText(path, json);
    }

public static PlayerData Load()
{
    string path = Application.persistentDataPath + "/savefile.json";
    if (File.Exists(path))
    {
        string json = File.ReadAllText(path);
        
        // Verificăm dacă JSON-ul nu este gol sau doar spații
        if (string.IsNullOrEmpty(json) || json.Trim().Length < 2) 
        {
            Debug.LogWarning("Fisierul de salvare este gol!");
            return null;
        }

        try {
            return JsonUtility.FromJson<PlayerData>(json);
        } catch (System.Exception e) {
            Debug.LogError("Eroare la deserializare: " + e.Message);
            return null;
        }
    }
    return null;
}
}