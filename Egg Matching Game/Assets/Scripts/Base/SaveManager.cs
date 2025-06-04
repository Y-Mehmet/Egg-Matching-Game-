using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public int levelIndex=0;
    private string levelIndexKey = "LevelIndex";
    private void Awake()
    {
        // Ensure that there is only one instance of SaveManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
            Load(); // Load the saved level index when the game starts
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }
    public void Load()
    {
        // Load the saved level index from PlayerPrefs
        levelIndex = PlayerPrefs.GetInt(levelIndexKey, 0);
        Debug.Log("Loaded Level Index: " + levelIndex);
    }
    public void Save()
    {
        // Save the current level index to PlayerPrefs
        PlayerPrefs.SetInt(levelIndexKey, levelIndex);
        PlayerPrefs.Save();
        Debug.Log("Saved Level Index: " + levelIndex);
    }
    public void IncrementLevel()
    {
        levelIndex++;
        Save();
    }

    public void ResetLevel()
    {
        levelIndex = 0;
        Save();
    }
}
