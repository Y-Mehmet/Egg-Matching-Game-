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
            // E�er _instance daha �nce y�klenmemi�se (yani null ise)
            if (_instance == null)
            {
                // Resources klas�r�nden "ResourceDatabaseSO" isimli asset'i bul ve y�kle.
                // Not: Buradaki "ResourceDatabaseSO" string'i, Resources klas�r�ndeki
                // asset dosyan�z�n ad�yla ayn� olmal�d�r.
                _instance = Resources.Load<ResourceDatabaseSO>("ResourceDatabaseSO");

                // E�er y�kleme ba�ar�s�z olursa (dosya bulunamazsa veya ad� yanl��sa)
                // geli�tiriciyi bilgilendirmek i�in bir hata mesaj� g�ster.
                if (_instance == null)
                {
                    Debug.LogError("ResourceDatabaseSO.cs: Resources klas�r� i�inde 'ResourceDatabaseSO.asset' dosyas� bulunamad�!");
                }
            }
            return _instance;
        }
    }

    // --- VER�TABANI KISMI (Bu k�s�m ayn� kal�yor) ---

    [SerializeField] private List<Sprite> resourceSprites;

    public Sprite GetSprite(ResourceType type)
    {
        int index = (int)type;

        if (index >= 0 && index < resourceSprites.Count)
        {
            return resourceSprites[index];
        }

        Debug.LogError($"ResourceDatabase'de '{type}' i�in bir sprite bulunamad�! L�tfen 'ResourceDatabaseSO.asset' dosyas�n� kontrol edin.");
        return null;
    }
}