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
            DontDestroyOnLoad(gameObject); // Opsiyonel: Sahne de�i�imlerinde kaybolmas�n
            LoadResources(); // Kaynaklar� y�kle
        }
    }

    // --- Events (Actions) ---
    // Bir kayna��n miktar� de�i�ti�inde bu event'ler tetiklenir.
    // UI elemanlar� bu event'leri dinleyerek kendilerini g�ncelleyebilir.
    public event Action<ResourceType, int> OnResourceChanged;

    // --- Resource Data ---
    private int coins;
    private int gems;
    private int energy;

    // --- Public Methods ---

    /// <summary>
    /// Belirtilen t�rdeki kayna��n mevcut miktar�n� d�nd�r�r.
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
    /// Belirtilen t�rdeki kayna�a belirtilen miktarda ekler.
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

        // De�i�ikli�i dinleyen herkese haber ver.
        OnResourceChanged?.Invoke(type, GetResourceAmount(type));
        SaveResources(); // De�i�ikli�i kaydet
    }

    /// <summary>
    /// Belirtilen t�rdeki kaynaktan belirtilen miktarda harcamaya �al���r.
    /// </summary>
    /// <returns>Harcama ba�ar�l�ysa true, yeterli kaynak yoksa false d�ner.</returns>
    public bool SpendResource(ResourceType type, int amount)
    {
        if (amount <= 0) return true; // 0 veya eksi harcama her zaman ba�ar�l�d�r.

        if (HasEnoughResource(type, amount))
        {
            switch (type)
            {
                case ResourceType.Coin: coins -= amount; break;
                case ResourceType.Gem: gems -= amount; break;
                case ResourceType.Energy: energy -= amount; break;
            }

            // De�i�ikli�i dinleyen herkese haber ver.
            OnResourceChanged?.Invoke(type, GetResourceAmount(type));
            SaveResources(); // De�i�ikli�i kaydet
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
    /// Belirtilen t�rdeki kaynaktan yeterli miktarda olup olmad���n� kontrol eder.
    /// </summary>
    public bool HasEnoughResource(ResourceType type, int amount)
    {
        return GetResourceAmount(type) >= amount;
    }


    // --- Saving & Loading ---
    // Kaynaklar� PlayerPrefs veya ba�ka bir kay�t sistemine kaydeder/y�kler.
    private void SaveResources()
    {
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.SetInt("PlayerGems", gems);
        PlayerPrefs.SetInt("PlayerEnergy", energy);
        PlayerPrefs.Save();
    }

    private void LoadResources()
    {
        coins = PlayerPrefs.GetInt("PlayerCoins", 100); // Ba�lang��ta 100 coin verelim.
        gems = PlayerPrefs.GetInt("PlayerGems", 10);
        energy = PlayerPrefs.GetInt("PlayerEnergy", 50);

        // Y�kleme sonras� UI'� g�ncellemek i�in event'leri tetikle
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