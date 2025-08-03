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
    private int playCount; // Oyun oynama sayýsý, SaveGameData'da da var
    private int levelIndex ; // Oyun seviyesini tutan deðiþken
    public int coinsPerGame = 50; // Her oyun sonunda kazanýlan para miktarý
    public int gemsPerGame = 1; // Her oyun sonunda kazanýlan elmas miktarý
    public int DragonIndex = 0;
    public int macDragobIndex = 9;
    public bool isPushAlarmEnabled = false;
    public float soundFxVolume = 0.8f;
    public float musicVolume = 0.8f;
    public bool isVibrationEnabled = false;
    public int SelectedDragonIndex = -1;
    public bool isTutorial = true;
    public int time = 45;


    // Yüklenen tüm oyun verilerini tutan referansýmýz.
    private SaveGameData gameData;
    // --- Enerji Yenileme Sistemi ---
    [Header("Enerji Yenileme Ayarlarý")]
    public int maxEnergy = 5; // Inspector'dan ayarlanabilir maksimum enerji
    private DateTime nextEnergyTime; // Bir sonraki enerjinin verileceði zaman
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
            LoadResources(); // Kaynaklarý SaveSystem üzerinden yükle
        }
    }
    private void OnEnable()
    {
        currentRewardedTypeChanged += CurrentRewaredTypeChange;
    }
    private void OnDisable()
    {
        currentRewardedTypeChanged -= CurrentRewaredTypeChange; // Dinleyiciyi kaldýr
    }

    private void Start()
    {
        // Enerji yenileme döngüsünü baþlat
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

    // Oyuncu oyundan çýktýðýnda son bir kez kaydetmek için.
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
            case ResourceType.PlayCount: return playCount; // Oyun oynama sayýsýný döndürüyoruz
            case ResourceType.LevelIndex: return levelIndex; // Oyun seviyesini döndürüyoruz
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
                if (energy > maxEnergy) energy = maxEnergy; // Enerji max deðeri geçemez
                break;
            case ResourceType.PlayCount: playCount += amount; break; // Oyun oynama sayýsýný artýrýyoruz
            case ResourceType.LevelIndex: levelIndex += amount; break; // Oyun seviyesini artýrýyoruz
            case ResourceType.DragonIndex: DragonIndex += amount; if (DragonIndex > macDragobIndex) DragonIndex = macDragobIndex; break; // Ejderha indeksini artýrýyoruz
            case ResourceType.Time:time += amount;break;       
        }

        OnResourceChanged?.Invoke(type, GetResourceAmount(type));
        SaveResources(); // Deðiþikliði yeni sistemle kaydet
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        Debug.LogWarning("type: " + type + ", amount: " + amount + ", current amount: " + GetResourceAmount(type));
        if (amount <= 0) return true;

        if (HasEnoughResource(type, amount))
        {
            // Enerji harcamadan önce tam dolu olup olmadýðýný kontrol et
            bool wasEnergyMaxed = (type == ResourceType.Energy && this.energy == maxEnergy);

            switch (type)
            {
                case ResourceType.Coin: coins -= amount; break;
                case ResourceType.Gem: gems -= amount; break;
                case ResourceType.Energy: energy -= amount; break;
            }

            // Eðer enerji harcandýysa ve daha önce tam doluysa, sayacý þimdi baþlat.
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
            Debug.LogError("gameData referansý boþ, kayýt iþlemi baþarýsýz oldu. Oyunun doðru baþlatýldýðýndan emin olun.");
            return;
        }

        // 1. ResourceManager'daki güncel deðerleri gameData objesine yaz.
        gameData.coins = this.coins;
        gameData.gems = this.gems;
        gameData.energy = this.energy;
        gameData.playCount = this.playCount; // Oyun oynama sayýsýný da kaydediyoruz
        gameData.levelIndex = this.levelIndex; // Oyun seviyesini de kaydediyoruz
        gameData.DragonIndex = this.DragonIndex;
        gameData.isVibrationEnabled= this.isVibrationEnabled;
        gameData.musicVolume = this.musicVolume;
        gameData.soundFxVolume = this.soundFxVolume;
        gameData.isPushAlarmEnabled = this.isPushAlarmEnabled;
        gameData.SelectedDragonIndex=this.SelectedDragonIndex;
        gameData.isTutorial=this.isTutorial;

        // YENÝ: Zaman bilgisini kaydet
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

        // YENÝ: Zaman bilgisini yükle
        if (!string.IsNullOrEmpty(gameData.nextEnergyTimeString))
        {
            long tempTime = Convert.ToInt64(gameData.nextEnergyTimeString);
            this.nextEnergyTime = DateTime.FromBinary(tempTime);
        }
        else if (this.energy < maxEnergy)
        {
            // Eðer kayýtlý zaman yoksa ve enerji max deðilse, yeni bir zamanlayýcý baþlat.
            this.nextEnergyTime = GetUTCNow().Add(energyRechargeDuration);
        }

        // YENÝ: Çevrimdýþý kazanýmý kontrol et
        CheckOfflineEnergyGain();

        // UI ve diðer sistemleri bilgilendir
        OnResourceChanged?.Invoke(ResourceType.Coin, this.coins);
        OnResourceChanged?.Invoke(ResourceType.Gem, this.gems);
        OnResourceChanged?.Invoke(ResourceType.Energy, this.energy);

        Debug.Log("Kaynaklar yüklendi ve çevrimdýþý enerji kazanýmý hesaplandý.");
    }

    /// <summary>
    /// Oyuncu oyunda deðilken geçen süreyi hesaplar ve hak edilen enerjiyi verir.
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
                    // Tam olarak dolmadýysa, bir sonraki dolum zamanýný güncelle.
                    DateTime newNextTime = nextEnergyTime.AddMinutes(energyGained * energyRechargeDuration.TotalMinutes);
                    nextEnergyTime = newNextTime;
                }
                OnResourceChanged?.Invoke(ResourceType.Energy, energy);
            }
        }
    }

    private IEnumerator RechargeEnergyCoroutine()
    {
        // Oyun baþlangýcýnda, eðer çevrimdýþý kazanýmdan sonra hala enerji max deðilse sayacý baþlat.
        yield return new WaitForSeconds(1f); // Diðer sistemlerin yüklenmesi için kýsa bir bekleme
        StartEnergyRechargeTimerIfNeeded();

        while (true)
        {
            // Her saniye kontrol etmek yerine 5 saniyede bir kontrol etmek performansý artýrabilir.
            yield return new WaitForSeconds(5f);

            if (energy < maxEnergy)
            {
                // Zaman doldu mu kontrol et
                if (GetUTCNow() >= nextEnergyTime)
                {
                    Debug.Log("Enerji yenileme zamaný geldi. +1 Enerji eklendi.");
                    energy++; // Direkt olarak enerjiyi artýrýyoruz. AddResource'a gerek yok çünkü tüm kontroller burada.

                    // Enerji eklendikten sonra hala maksimum seviyenin altýndaysak, yeni bir sayaç baþlat.
                    if (energy < maxEnergy)
                    {
                        nextEnergyTime = GetUTCNow().Add(energyRechargeDuration);
                        Debug.Log($"Enerji hala max deðil. Yeni sayaç baþlatýldý. Sonraki zaman: {nextEnergyTime}");
                    }
                    else
                    {
                        Debug.Log("Enerji maksimuma ulaþtý. Sayaç durduruldu.");
                    }

                    OnResourceChanged?.Invoke(ResourceType.Energy, energy);
                    SaveResources(); // Deðiþikliði kaydet
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

    }// Bu metotlarý ResourceManager.cs dosyanýzýn içine ekleyin

    /// <summary>
    /// Bir sonraki enerjinin gelmesine kalan süreyi TimeSpan nesnesi olarak döndürür.
    /// Enerji doluysa veya sayaç aktif deðilse TimeSpan.Zero döndürür.
    /// </summary>
    /// <returns>Kalan süreyi temsil eden bir TimeSpan nesnesi.</returns>
    public TimeSpan GetTimeToNextEnergy()
    {
        // Eðer enerji zaten maksimum seviyedeyse, bekleme süresi yoktur.
        if (energy >= maxEnergy)
        {
            return TimeSpan.Zero;
        }

        // Þu anki UTC zamanýný al.
        DateTime now = GetUTCNow();

        // Kalan süreyi hesapla.
        TimeSpan timeRemaining = nextEnergyTime - now;

        // Eðer süre geçmiþse (negatifse), sýfýr döndür.
        if (timeRemaining.TotalSeconds <= 0)
        {
            return TimeSpan.Zero;
        }

        return timeRemaining;
    }

    /// <summary>
    /// Bir sonraki enerji için geri sayým metnini "dk:sn" formatýnda döndürür.
    /// Enerji doluysa "Dolu" yazar.
    /// </summary>
    /// <returns>Biçimlendirilmiþ geri sayým metni.</returns>
    public string GetNextEnergyCountdownText()
    {
        TimeSpan timeRemaining = GetTimeToNextEnergy();

        // Eðer kalan süre sýfýr ise, enerji ya doludur ya da tam þimdi dolmuþtur.
        if (timeRemaining == TimeSpan.Zero)
        {
            // Enerji tam ise "Dolu" yaz, deðilse yeni enerji gelmek üzere olduðu için kýsa bir bekleme metni gösterilebilir.
            return energy >= maxEnergy ? "Full" : "00:00";
        }

        // Kalan süreyi "dakika:saniye" formatýna çevir. Örnek: "29:59"
        return timeRemaining.ToString(@"mm\:ss");
    }
    /// <summary>
    /// Enerji maksimumun altýndaysa ve zamanlayýcý zaten gelecekte bir zamana ayarlý deðilse,
    /// yeni bir enerji yenileme sayacý baþlatýr.
    /// </summary>
    private void StartEnergyRechargeTimerIfNeeded()
    {
        // Sadece enerji max'ýn altýndaysa ve zamanlayýcý geçmiþte kalmýþsa (yani aktif bir sayým yoksa) tetiklenir.
        if (energy < maxEnergy && GetUTCNow() >= nextEnergyTime)
        {
            nextEnergyTime = GetUTCNow().Add(energyRechargeDuration);
            Debug.Log($"Enerji sayacý baþlatýldý. Sonraki enerji zamaný: {nextEnergyTime}");
            SaveResources(); // Zamanlayýcý baþladýðýnda durumu kaydet
        }
    }
    public void IncraseLevelData()
    {


        levelIndex++;
        GameManager.instance.Save();
    }
    /// <summary>
    /// Zaman dilimi sorunlarýný önlemek için her zaman UTC zamanýný kullanýr.
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