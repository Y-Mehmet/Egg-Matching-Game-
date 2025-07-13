using System;
using System.Collections;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static ResourceManager Instance { get; private set; }
    public Action<ResourceType, int> OnResourceChanged;

    private int coins;
    private int gems;
    private int energy;
    private int playCount; // Oyun oynama sayýsý, SaveGameData'da da var
    public int coinsPerGame = 40; // Her oyun sonunda kazanýlan para miktarý
    public int gemsPerGame = 1; // Her oyun sonunda kazanýlan elmas miktarý

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

    private void Start()
    {
        // Enerji yenileme döngüsünü baþlat
        StartCoroutine(RechargeEnergyCoroutine());
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
        }

        OnResourceChanged?.Invoke(type, GetResourceAmount(type));
        SaveResources(); // Deðiþikliði yeni sistemle kaydet
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        if (amount <= 0) return true;

        if (HasEnoughResource(type, amount))
        {
            switch (type)
            {
                case ResourceType.Coin: coins -= amount; break;
                case ResourceType.Gem: gems -= amount; break;
                case ResourceType.Energy: energy -= amount; break;

            }

            OnResourceChanged?.Invoke(type, GetResourceAmount(type));
            SaveResources(); // Deðiþikliði yeni sistemle kaydet
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

    // --- Yeni Kayýt ve Yükleme Metotlarý ---

    private void SaveResources()
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

        // YENÝ: Zaman bilgisini kaydet
        gameData.nextEnergyTimeString = this.nextEnergyTime.ToBinary().ToString();

        SaveSystem.Save(gameData);
    }

    private void LoadResources()
    {
        gameData = SaveSystem.Load();

        this.coins = gameData.coins;
        this.gems = gameData.gems;
        this.energy = gameData.energy;
        this.playCount = gameData.playCount;

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
            }
        }
    }

    /// <summary>
    /// Oyuncu oyundayken enerjinin dolmasýný saðlayan arkaplan döngüsü.
    /// </summary>
    private IEnumerator RechargeEnergyCoroutine()
    {
        while (true)
        {
            // Her saniye kontrol et
            yield return new WaitForSeconds(1f);

            if (energy < maxEnergy)
            {
                if (GetUTCNow() >= nextEnergyTime)
                {
                    AddResource(ResourceType.Energy, 1);

                    // Eðer hala max enerjinin altýndaysak, bir sonraki zamanlayýcýyý kur.
                    if (energy < maxEnergy)
                    {
                        nextEnergyTime = GetUTCNow().Add(energyRechargeDuration);
                    }

                    SaveResources(); // Deðiþikliði kaydet
                }
            }
        }
    }
    public void GetReweard()
    {
        if (GameManager.instance.currentRewarded == RewardedType.Resource)
        {
            AddResource(ResourceType.Coin, coinsPerGame * 3);
            AddResource(ResourceType.Gem, gemsPerGame * 3);
        }
        else if (GameManager.instance.currentRewarded == RewardedType.Energy)
        {
            AddResource(ResourceType.Energy, 1);
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
    /// Zaman dilimi sorunlarýný önlemek için her zaman UTC zamanýný kullanýr.
    /// </summary>
    private DateTime GetUTCNow()
    {
        return DateTime.UtcNow;

    }
}
    public enum ResourceType
{
    Coin,
    Gem,
    Energy,
    PlayCount
}