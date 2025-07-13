[System.Serializable]
public class SaveGameData
{
    // Mevcut veriler
    public int levelIndex = 0;
    public bool isFirstLaunch = true;

    // --- YENİ EKLENEN ALANLAR ---
    // ResourceManager'daki başlangıç değerleriyle aynı yapalım.
    public int coins = 100;
    public int gems = 0;
    public int energy = 5;
    public int playCount = 0;

    public void IncraseLevelData()
    {
        levelIndex++;
    }
}