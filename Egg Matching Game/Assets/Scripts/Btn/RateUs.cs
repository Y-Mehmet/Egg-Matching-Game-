using UnityEngine;

public class RateUs : MonoBehaviour
{
    // Inspector'dan kolayca de�i�tirebilmek i�in linki bir de�i�kene atayal�m.
    [Tooltip("Uygulaman�n tam Google Play Store URL'si")]
    [SerializeField]
    private string googlePlayURL = "https://play.google.com/store/apps/details?id=com.MYGame.com.EggMatching&utm_source=emea_Med";

    /// <summary>
    /// Butona t�kland���nda Google Play Store sayfas�n� a�ar.
    /// </summary>
    public void OpenStorePage()
    {
        // Linkin bo� olup olmad���n� kontrol etmek her zaman iyi bir pratiktir.
        if (string.IsNullOrEmpty(googlePlayURL))
        {
            Debug.LogError("Google Play URL'si Inspector'da ayarlanmam��!");
            return;
        }

        // Belirtilen URL'yi a�ar.
        Application.OpenURL(googlePlayURL);
        Debug.Log("Ma�aza sayfas� a��l�yor: " + googlePlayURL);
    }

}
