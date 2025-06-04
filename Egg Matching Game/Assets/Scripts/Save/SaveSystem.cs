using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string filePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveGameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
        Debug.Log("Oyun verisi kaydedildi.");
    }

    public static SaveGameData Load()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("Ýlk açýlýþ - kayýt bulunamadý.");
            SaveGameData defaultData = new SaveGameData();
            Save(defaultData); // Ýlk veri kaydý
            return defaultData;
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<SaveGameData>(json);
    }
    
}
