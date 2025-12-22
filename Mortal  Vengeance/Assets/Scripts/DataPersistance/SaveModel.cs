using System.IO;
using UnityEngine;

public static class SaveModel
{
    static string path = Application.persistentDataPath + "/save.json";

    public static void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Saved to: " + path);
    }

    public static PlayerData Load()
    {
        if (!File.Exists(path))
        {
            Debug.Log("No save file found");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<PlayerData>(json);
    }
}
