using System.Diagnostics;

[System.Serializable]
public class SaveGameData
{
    // Mevcut veriler
    public int levelIndex = 0;
    public int DragonIndex = 0;
    public bool isFirstLaunch = true;

    // --- YENİ EKLENEN ALANLAR ---
    // ResourceManager'daki başlangıç değerleriyle aynı yapalım.
    public int coins = 100;
    public int gems = 0;
    public int energy = 5;
    public int playCount = 0;
    public string nextEnergyTimeString=string.Empty;
    public bool isPushAlarmEnabled = true;
    public float soundFxVolume = 0.5f;
    public float musicVolume = 0.5f;
    public bool isVibrationEnabled = true;
    public int SelectedDragonIndex = -1;


}