using UnityEngine;

public class RateUs : MonoBehaviour
{
    // Inspector'dan kolayca deðiþtirebilmek için linki bir deðiþkene atayalým.
    [Tooltip("Uygulamanýn tam Google Play Store URL'si")]
    [SerializeField]
    private string googlePlayURL = "https://play.google.com/store/apps/details?id=com.MYGame.com.EggMatching&utm_source=emea_Med";

    /// <summary>
    /// Butona týklandýðýnda Google Play Store sayfasýný açar.
    /// </summary>
    public void OpenStorePage()
    {
        // Linkin boþ olup olmadýðýný kontrol etmek her zaman iyi bir pratiktir.
        if (string.IsNullOrEmpty(googlePlayURL))
        {
            Debug.LogError("Google Play URL'si Inspector'da ayarlanmamýþ!");
            return;
        }

        // Belirtilen URL'yi açar.
        Application.OpenURL(googlePlayURL);
        Debug.Log("Maðaza sayfasý açýlýyor: " + googlePlayURL);
    }

}
