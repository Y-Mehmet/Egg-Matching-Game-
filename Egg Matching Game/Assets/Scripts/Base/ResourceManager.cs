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
    private int playCount; // Oyun oynama say�s�, SaveGameData'da da var
    public int coinsPerGame = 40; // Her oyun sonunda kazan�lan para miktar�
    public int gemsPerGame = 1; // Her oyun sonunda kazan�lan elmas miktar�

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

    private void Start()
    {
        // Enerji yenileme d�ng�s�n� ba�lat
        StartCoroutine(RechargeEnergyCoroutine());
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
        }

        OnResourceChanged?.Invoke(type, GetResourceAmount(type));
        SaveResources(); // De�i�ikli�i yeni sistemle kaydet
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
            SaveResources(); // De�i�ikli�i yeni sistemle kaydet
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

    // --- Yeni Kay�t ve Y�kleme Metotlar� ---

    private void SaveResources()
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

        // YEN�: Zaman bilgisini kaydet
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
            }
        }
    }

    /// <summary>
    /// Oyuncu oyundayken enerjinin dolmas�n� sa�layan arkaplan d�ng�s�.
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

                    // E�er hala max enerjinin alt�ndaysak, bir sonraki zamanlay�c�y� kur.
                    if (energy < maxEnergy)
                    {
                        nextEnergyTime = GetUTCNow().Add(energyRechargeDuration);
                    }

                    SaveResources(); // De�i�ikli�i kaydet
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
    /// Zaman dilimi sorunlar�n� �nlemek i�in her zaman UTC zaman�n� kullan�r.
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