using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static ResourceManager Instance { get; private set; }
    public Action<ResourceType, int> OnResourceChanged;

    private int coins;
    private int gems;
    private int energy;

    // Yüklenen tüm oyun verilerini tutan referansýmýz.
    private SaveGameData gameData;

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
            case ResourceType.Energy: energy += amount; break;
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

        // 2. Güncellenmiþ gameData objesini dosyaya kaydet.
        SaveSystem.Save(gameData);
    }

    private void LoadResources()
    {
        // 1. SaveSystem'den tüm oyun verisini yükle.
        gameData = SaveSystem.Load();

        // 2. Yüklenen veriyi ResourceManager'daki alanlara ata.
        this.coins = gameData.coins;
        this.gems = gameData.gems;
        this.energy = gameData.energy;

        // 3. UI ve diðer sistemleri bilgilendir.
        OnResourceChanged?.Invoke(ResourceType.Coin, this.coins);
        OnResourceChanged?.Invoke(ResourceType.Gem, this.gems);
        OnResourceChanged?.Invoke(ResourceType.Energy, this.energy);

        Debug.Log("Kaynaklar merkezi SaveSystem'den baþarýyla yüklendi.");
    }
}
// ResourceType.cs
public enum ResourceType
{
    Coin,
    Gem,
    Energy
}