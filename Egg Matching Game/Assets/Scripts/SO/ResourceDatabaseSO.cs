using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDatabaseSO", menuName = "Game/Resource Database")]
public class ResourceDatabaseSO : ScriptableObject
{
    [Header("Coin,\r\n    Gem,\r\n    Energy,\r\n    PlayCount,\r\n    LevelIndex,")]

    private static ResourceDatabaseSO _instance;

    public static ResourceDatabaseSO Instance
    {
        get
        {
            // Eðer _instance daha önce yüklenmemiþse (yani null ise)
            if (_instance == null)
            {
                // Resources klasöründen "ResourceDatabaseSO" isimli asset'i bul ve yükle.
                // Not: Buradaki "ResourceDatabaseSO" string'i, Resources klasöründeki
                // asset dosyanýzýn adýyla ayný olmalýdýr.
                _instance = Resources.Load<ResourceDatabaseSO>("ResourceDatabaseSO");

                // Eðer yükleme baþarýsýz olursa (dosya bulunamazsa veya adý yanlýþsa)
                // geliþtiriciyi bilgilendirmek için bir hata mesajý göster.
                if (_instance == null)
                {
                    Debug.LogError("ResourceDatabaseSO.cs: Resources klasörü içinde 'ResourceDatabaseSO.asset' dosyasý bulunamadý!");
                }
            }
            return _instance;
        }
    }

    // --- VERÝTABANI KISMI (Bu kýsým ayný kalýyor) ---

    [SerializeField] private List<Sprite> resourceSprites;

    public Sprite GetSprite(ResourceType type)
    {
        int index = (int)type;

        if (index >= 0 && index < resourceSprites.Count)
        {
            return resourceSprites[index];
        }

        Debug.LogError($"ResourceDatabase'de '{type}' için bir sprite bulunamadý! Lütfen 'ResourceDatabaseSO.asset' dosyasýný kontrol edin.");
        return null;
    }
}