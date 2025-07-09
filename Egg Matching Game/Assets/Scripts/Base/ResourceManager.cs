// ResourceManager.cs
using Mono.Cecil;
using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static ResourceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opsiyonel: Sahne deðiþimlerinde kaybolmasýn
            LoadResources(); // Kaynaklarý yükle
        }
    }

    // --- Events (Actions) ---
    // Bir kaynaðýn miktarý deðiþtiðinde bu event'ler tetiklenir.
    // UI elemanlarý bu event'leri dinleyerek kendilerini güncelleyebilir.
    public event Action<ResourceType, int> OnResourceChanged;

    // --- Resource Data ---
    private int coins;
    private int gems;
    private int energy;

    // --- Public Methods ---

    /// <summary>
    /// Belirtilen türdeki kaynaðýn mevcut miktarýný döndürür.
    /// </summary>
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

    /// <summary>
    /// Belirtilen türdeki kaynaða belirtilen miktarda ekler.
    /// </summary>
    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0) return;

        switch (type)
        {
            case ResourceType.Coin: coins += amount; break;
            case ResourceType.Gem: gems += amount; break;
            case ResourceType.Energy: energy += amount; break;
        }

        // Deðiþikliði dinleyen herkese haber ver.
        OnResourceChanged?.Invoke(type, GetResourceAmount(type));
        SaveResources(); // Deðiþikliði kaydet
    }

    /// <summary>
    /// Belirtilen türdeki kaynaktan belirtilen miktarda harcamaya çalýþýr.
    /// </summary>
    /// <returns>Harcama baþarýlýysa true, yeterli kaynak yoksa false döner.</returns>
    public bool SpendResource(ResourceType type, int amount)
    {
        if (amount <= 0) return true; // 0 veya eksi harcama her zaman baþarýlýdýr.

        if (HasEnoughResource(type, amount))
        {
            switch (type)
            {
                case ResourceType.Coin: coins -= amount; break;
                case ResourceType.Gem: gems -= amount; break;
                case ResourceType.Energy: energy -= amount; break;
            }

            // Deðiþikliði dinleyen herkese haber ver.
            OnResourceChanged?.Invoke(type, GetResourceAmount(type));
            SaveResources(); // Deðiþikliði kaydet
            return true;
        }
        else
        {
            // Yeterli kaynak yok.
            Debug.LogWarning($"Not enough {type} to spend. Required: {amount}, Have: {GetResourceAmount(type)}");
            return false;
        }
    }

    /// <summary>
    /// Belirtilen türdeki kaynaktan yeterli miktarda olup olmadýðýný kontrol eder.
    /// </summary>
    public bool HasEnoughResource(ResourceType type, int amount)
    {
        return GetResourceAmount(type) >= amount;
    }


    // --- Saving & Loading ---
    // Kaynaklarý PlayerPrefs veya baþka bir kayýt sistemine kaydeder/yükler.
    private void SaveResources()
    {
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.SetInt("PlayerGems", gems);
        PlayerPrefs.SetInt("PlayerEnergy", energy);
        PlayerPrefs.Save();
    }

    private void LoadResources()
    {
        coins = PlayerPrefs.GetInt("PlayerCoins", 100); // Baþlangýçta 100 coin verelim.
        gems = PlayerPrefs.GetInt("PlayerGems", 10);
        energy = PlayerPrefs.GetInt("PlayerEnergy", 50);

        // Yükleme sonrasý UI'ý güncellemek için event'leri tetikle
        OnResourceChanged?.Invoke(ResourceType.Coin, coins);
        OnResourceChanged?.Invoke(ResourceType.Gem, gems);
        OnResourceChanged?.Invoke(ResourceType.Energy, energy);
    }
}
// ResourceType.cs
public enum ResourceType
{
    Coin,
    Gem,
    Energy
}