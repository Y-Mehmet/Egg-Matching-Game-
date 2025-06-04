[System.Serializable]
public class SaveGameData
{
    public int levelIndex = 0;
    public bool isFirstLaunch = true;
    public void IncraseLevelData()
    {
        levelIndex++;
    }
}