using System;
using System.Collections;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static ResourceManager Instance { get; private set; }
    public Action onSelectedDragonIndexChanged;
    public Action<ResourceType, int> OnResourceChanged;
    public Action<RewardedType> currentRewardedTypeChanged;
    public Action onIAPchanged;
    public RewardedType currentRewarded = RewardedType.Resource;

    private int coins;
    private int gems;
    private int energy;
    private int playCount; // Oyun oynama say�s�, SaveGameData'da da var
    private int levelIndex ; // Oyun seviyesini tutan de�i�ken
    public int coinsPerGame = 50; // Her oyun sonunda kazan�lan para miktar�
    public int gemsPerGame = 1; // Her oyun sonunda kazan�lan elmas miktar�
    public int DragonIndex = 0;
    public int macDragobIndex = 9;
    public bool isPushAlarmEnabled = false;
    public float soundFxVolume = 0.8f;
    public float musicVolume = 0.8f;
    public bool isVibrationEnabled = false;
    public int SelectedDragonIndex = -1;
    public bool isTutorial = true;
    public int time = 45;


    // Y�klenen t�m oyun verilerini tutan referans�m�z.
    private SaveGameData gameData;
    // --- Enerji Yenileme Sistemi ---
    [Header("Enerji Yenileme Ayarlar�")]
    public int maxEnergy = 5; // Inspector'dan ayarlanabilir maksimum enerji
    private DateTime nextEnergyTime; // Bir sonraki enerjinin verilece�i zaman
    private readonly TimeSpan energyRechargeDuration = TimeSpan.FromMinutes(30); // 30 dakika

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadResources(); // Kaynaklar� SaveSystem �zerinden y�kle
        }
    }
    private void OnEnable()
    {
        currentRewardedTypeChanged += CurrentRewaredTypeChange;
    }
    private void OnDisable()
    {
        currentRewardedTypeChanged -= CurrentRewaredTypeChange; // Dinleyiciyi kald�r
    }

    private void Start()
    {
        // Enerji yenileme d�ng�s�n� ba�lat
        StartCoroutine(RechargeEnergyCoroutine());
    }
    public void SetSoundResoruce(bool isPushAlarmEnabled, float soundFxVolume, float musicVolume, bool isVibrationEnabled)
    {
        this.isPushAlarmEnabled = isPushAlarmEnabled;
        this.soundFxVolume = soundFxVolume;
        this.musicVolume = musicVolume;
        this.isVibrationEnabled = isVibrationEnabled;
        SaveResources();
    }
    private void CurrentRewaredTypeChange(RewardedType type)
    {
        currentRewarded = type;
    }

    // Oyuncu oyundan ��kt���nda son bir kez kaydetmek i�in.
    private void OnApplicationQuit()
    {
        SaveResources();
    }
    public int GetResourceAmount(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Coin: return coins;
            case ResourceType.Gem: return gems;
            case ResourceType.Energy: return energy;
            case ResourceType.PlayCount: return playCount; // Oyun oynama say�s�n� d�nd�r�yoruz
            case ResourceType.LevelIndex: return levelIndex; // Oyun seviyesini d�nd�r�yoruz
            case ResourceType.DragonIndex: return DragonIndex;
            default: return 0;
        }
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0) return;

        switch (type)
        {
            case ResourceType.Coin: coins += amount; break;
            case ResourceType.Gem: gems += amount; break;
            case ResourceType.Energy:
                energy += amount;
                if (energy > maxEnergy) energy = maxEnergy; // Enerji max de�eri ge�emez
                break;
            case ResourceType.PlayCount: playCount += amount; break; // Oyun oynama say�s�n� art�r�yoruz
            case ResourceType.LevelIndex: levelIndex += amount; break; // Oyun seviyesini art�r�yoruz
            case ResourceType.DragonIndex: DragonIndex += amount; if (DragonIndex > macDragobIndex) DragonIndex = macDragobIndex; break; // Ejderha indeksini art�r�yoruz
            case ResourceType.Time:time += amount;break;       
        }

        OnResourceChanged?.Invoke(type, GetResourceAmount(type));
        SaveResources(); // De�i�ikli�i yeni sistemle kaydet
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        Debug.LogWarning("type: " + type + ", amount: " + amount + ", current amount: " + GetResourceAmount(type));
        if (amount <= 0) return true;

        if (HasEnoughResource(type, amount))
        {
            // Enerji harcamadan �nce tam dolu olup olmad���n� kontrol et
            bool wasEnergyMaxed = (type == ResourceType.Energy && this.energy == maxEnergy);

            switch (type)
            {
                case ResourceType.Coin: coins -= amount; break;
                case ResourceType.Gem: gems -= amount; break;
                case ResourceType.Energy: energy -= amount; break;
            }

            // E�er enerji harcand�ysa ve daha �nce tam doluysa, sayac� �imdi ba�lat.
            if (wasEnergyMaxed && this.energy < maxEnergy)
            {
                StartEnergyRechargeTimerIfNeeded();
            }

            OnResourceChanged?.Invoke(type, GetResourceAmount(type));
            SaveResources();
            return true;
        }
        else
        {
            Debug.LogWarning($"Yeterli {type} yok. Gerekli: {amount}, Mevcut: {GetResourceAmount(type)}");
            return false;
        }
    }

    public bool HasEnoughResource(ResourceType type, int amount)
    {
        return GetResourceAmount(type) >= amount;
    }

   

    public  void SaveResources()
    {
        if (gameData == null)
        {
            Debug.LogError("gameData referans� bo�, kay�t i�lemi ba�ar�s�z oldu. Oyunun do�ru ba�lat�ld���ndan emin olun.");
            return;
        }

        // 1. ResourceManager'daki g�ncel de�erleri gameData objesine yaz.
        gameData.coins = this.coins;
        gameData.gems = this.gems;
        gameData.energy = this.energy;
        gameData.playCount = this.playCount; // Oyun oynama say�s�n� da kaydediyoruz
        gameData.levelIndex = this.levelIndex; // Oyun seviyesini de kaydediyoruz
        gameData.DragonIndex = this.DragonIndex;
        gameData.isVibrationEnabled= this.isVibrationEnabled;
        gameData.musicVolume = this.musicVolume;
        gameData.soundFxVolume = this.soundFxVolume;
        gameData.isPushAlarmEnabled = this.isPushAlarmEnabled;
        gameData.SelectedDragonIndex=this.SelectedDragonIndex;
        gameData.isTutorial=this.isTutorial;

        // YEN�: Zaman bilgisini kaydet
        gameData.nextEnergyTimeString = this.nextEnergyTime.ToBinary().ToString();
        gameData.time = this.time;

        SaveSystem.Save(gameData);
    }

    private void LoadResources()
    {
        gameData = SaveSystem.Load();

        this.coins = gameData.coins;
        this.gems = gameData.gems;
        this.energy = gameData.energy;
        this.playCount = gameData.playCount;
        this.levelIndex = gameData.levelIndex;
        this.DragonIndex = gameData.DragonIndex;
        this.isPushAlarmEnabled = gameData.isPushAlarmEnabled;
        this.soundFxVolume = gameData.soundFxVolume;
        this.musicVolume = gameData.musicVolume;
        this.isVibrationEnabled = gameData.isVibrationEnabled;
        this.SelectedDragonIndex = gameData.SelectedDragonIndex;
        this.isTutorial = gameData.isTutorial;
        this.time = gameData.time;

        // YEN�: Zaman bilgisini y�kle
        if (!string.IsNullOrEmpty(gameData.nextEnergyTimeString))
        {
            long tempTime = Convert.ToInt64(gameData.nextEnergyTimeString);
            this.nextEnergyTime = DateTime.FromBinary(tempTime);
        }
        else if (this.energy < maxEnergy)
        {
            // E�er kay�tl� zaman yoksa ve enerji max de�ilse, yeni bir zamanlay�c� ba�lat.
            this.nextEnergyTime = GetUTCNow().Add(energyRechargeDuration);
        }

        // YEN�: �evrimd��� kazan�m� kontrol et
        CheckOfflineEnergyGain();

        // UI ve di�er sistemleri bilgilendir
        OnResourceChanged?.Invoke(ResourceType.Coin, this.coins);
        OnResourceChanged?.Invoke(ResourceType.Gem, this.gems);
        OnResourceChanged?.Invoke(ResourceType.Energy, this.energy);

        Debug.Log("Kaynaklar y�klendi ve �evrimd��� enerji kazan�m� hesapland�.");
    }

    /// <summary>
    /// Oyuncu oyunda de�ilken ge�en s�reyi hesaplar ve hak edilen enerjiyi verir.
    /// </summary>
    private void CheckOfflineEnergyGain()
    {
        if (energy >= maxEnergy) return;

        DateTime now = GetUTCNow();
        if (now > nextEnergyTime)
        {
            TimeSpan timePassed = now - nextEnergyTime;
            int energyGained = (int)(timePassed.TotalMinutes / energyRechargeDuration.TotalMinutes);

            if (energyGained > 0)
            {
                energy += energyGained;
                if (energy >= maxEnergy)
                {
                    energy = maxEnergy;
                }
                else
                {
                    // Tam olarak dolmad�ysa, bir sonraki dolum zaman�n� g�ncelle.
                    DateTime newNextTime = nextEnergyTime.AddMinutes(energyGained * energyRechargeDuration.TotalMinutes);
                    nextEnergyTime = newNextTime;
                }
                OnResourceChanged?.Invoke(ResourceType.Energy, energy);
            }
        }
    }

    private IEnumerator RechargeEnergyCoroutine()
    {
        // Oyun ba�lang�c�nda, e�er �evrimd��� kazan�mdan sonra hala enerji max de�ilse sayac� ba�lat.
        yield return new WaitForSeconds(1f); // Di�er sistemlerin y�klenmesi i�in k�sa bir bekleme
        StartEnergyRechargeTimerIfNeeded();

        while (true)
        {
            // Her saniye kontrol etmek yerine 5 saniyede bir kontrol etmek performans� art�rabilir.
            yield return new WaitForSeconds(5f);

            if (energy < maxEnergy)
            {
                // Zaman doldu mu kontrol et
                if (GetUTCNow() >= nextEnergyTime)
                {
                    Debug.Log("Enerji yenileme zaman� geldi. +1 Enerji eklendi.");
                    energy++; // Direkt olarak enerjiyi art�r�yoruz. AddResource'a gerek yok ��nk� t�m kontroller burada.

                    // Enerji eklendikten sonra hala maksimum seviyenin alt�ndaysak, yeni bir saya� ba�lat.
                    if (energy < maxEnergy)
                    {
                        nextEnergyTime = GetUTCNow().Add(energyRechargeDuration);
                        Debug.Log($"Enerji hala max de�il. Yeni saya� ba�lat�ld�. Sonraki zaman: {nextEnergyTime}");
                    }
                    else
                    {
                        Debug.Log("Enerji maksimuma ula�t�. Saya� durduruldu.");
                    }

                    OnResourceChanged?.Invoke(ResourceType.Energy, energy);
                    SaveResources(); // De�i�ikli�i kaydet
                }
            }
        }
    }
    public void GetReweard()
    {
        if (ResourceManager.Instance.currentRewarded == RewardedType.Resource)
        {
            AddResource(ResourceType.Coin, coinsPerGame * 3);
            AddResource(ResourceType.Gem, gemsPerGame * 3);
            OnResourceChanged?.Invoke(ResourceType.Coin, coins);
            OnResourceChanged?.Invoke(ResourceType.Gem, gems);

        }else if(currentRewarded==RewardedType.OneResource)
        {
            AddResource(ResourceType.Coin, coinsPerGame );
            AddResource(ResourceType.Gem, gemsPerGame );
            OnResourceChanged?.Invoke(ResourceType.Coin, coins);
            OnResourceChanged?.Invoke(ResourceType.Gem, gems);

        }
        else if (ResourceManager.Instance.currentRewarded == RewardedType.Energy)
        {
            AddResource(ResourceType.Energy, 1);
            OnResourceChanged?.Invoke(ResourceType.Energy, energy);
        }

    }// Bu metotlar� ResourceManager.cs dosyan�z�n i�ine ekleyin

    /// <summary>
    /// Bir sonraki enerjinin gelmesine kalan s�reyi TimeSpan nesnesi olarak d�nd�r�r.
    /// Enerji doluysa veya saya� aktif de�ilse TimeSpan.Zero d�nd�r�r.
    /// </summary>
    /// <returns>Kalan s�reyi temsil eden bir TimeSpan nesnesi.</returns>
    public TimeSpan GetTimeToNextEnergy()
    {
        // E�er enerji zaten maksimum seviyedeyse, bekleme s�resi yoktur.
        if (energy >= maxEnergy)
        {
            return TimeSpan.Zero;
        }

        // �u anki UTC zaman�n� al.
        DateTime now = GetUTCNow();

        // Kalan s�reyi hesapla.
        TimeSpan timeRemaining = nextEnergyTime - now;

        // E�er s�re ge�mi�se (negatifse), s�f�r d�nd�r.
        if (timeRemaining.TotalSeconds <= 0)
        {
            return TimeSpan.Zero;
        }

        return timeRemaining;
    }

    /// <summary>
    /// Bir sonraki enerji i�in geri say�m metnini "dk:sn" format�nda d�nd�r�r.
    /// Enerji doluysa "Dolu" yazar.
    /// </summary>
    /// <returns>Bi�imlendirilmi� geri say�m metni.</returns>
    public string GetNextEnergyCountdownText()
    {
        TimeSpan timeRemaining = GetTimeToNextEnergy();

        // E�er kalan s�re s�f�r ise, enerji ya doludur ya da tam �imdi dolmu�tur.
        if (timeRemaining == TimeSpan.Zero)
        {
            // Enerji tam ise "Dolu" yaz, de�ilse yeni enerji gelmek �zere oldu�u i�in k�sa bir bekleme metni g�sterilebilir.
            return energy >= maxEnergy ? "Full" : "00:00";
        }

        // Kalan s�reyi "dakika:saniye" format�na �evir. �rnek: "29:59"
        return timeRemaining.ToString(@"mm\:ss");
    }
    /// <summary>
    /// Enerji maksimumun alt�ndaysa ve zamanlay�c� zaten gelecekte bir zamana ayarl� de�ilse,
    /// yeni bir enerji yenileme sayac� ba�lat�r.
    /// </summary>
    private void StartEnergyRechargeTimerIfNeeded()
    {
        // Sadece enerji max'�n alt�ndaysa ve zamanlay�c� ge�mi�te kalm��sa (yani aktif bir say�m yoksa) tetiklenir.
        if (energy < maxEnergy && GetUTCNow() >= nextEnergyTime)
        {
            nextEnergyTime = GetUTCNow().Add(energyRechargeDuration);
            Debug.Log($"Enerji sayac� ba�lat�ld�. Sonraki enerji zaman�: {nextEnergyTime}");
            SaveResources(); // Zamanlay�c� ba�lad���nda durumu kaydet
        }
    }
    public void IncraseLevelData()
    {


        levelIndex++;
        GameManager.instance.Save();
    }
    /// <summary>
    /// Zaman dilimi sorunlar�n� �nlemek i�in her zaman UTC zaman�n� kullan�r.
    /// </summary>
    private DateTime GetUTCNow()
    {
        return DateTime.UtcNow;

    }
    public void Trade(int spendGemAmount, int addCoinAmount)
    {


        if (ResourceManager.Instance.SpendResource(ResourceType.Gem, spendGemAmount))
            ResourceManager.Instance.AddResource(ResourceType.Coin, addCoinAmount);


    }
}
    public enum ResourceType
{
    Coin,
    Gem,
    Energy,
    PlayCount,
    LevelIndex,
    DragonIndex,
    Time,
}