using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAds : MonoBehaviour
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iosAdUnitId;

    private string adUnitId;

    private void Awake()
    {
#if UNITY_IOS
                adUnitId = iosAdUnitId;
#elif UNITY_ANDROID
        adUnitId = androidAdUnitId;
#endif

        // Banner'ýn konumunu ayarla (örneðin ekranýn alt ortasý)
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);

        // Oyun baþladýðýnda veya bu script aktif olduðunda banner reklamý yükle
        LoadBannerAd();
    }

    public void LoadBannerAd()
    {
        // Banner reklam yükleme seçeneklerini belirle
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = BannerLoaded, // Reklam yüklendiðinde çaðrýlacak metod
            errorCallback = BannerLoadedError // Yükleme hatasý oluþtuðunda çaðrýlacak metod
        };

        // Banner reklamý belirtilen reklam birimi kimliði ile yükle
        Advertisement.Banner.Load(adUnitId, options);
    }

    public void ShowBannerAd()
    {
        // Banner reklam gösterme seçeneklerini belirle
        BannerOptions options = new BannerOptions
        {
            showCallback = BannerShown, // Reklam gösterildiðinde çaðrýlacak metod
            clickCallback = BannerClicked, // Reklama týklandýðýnda çaðrýlacak metod
            hideCallback = BannerHidden // Reklam gizlendiðinde çaðrýlacak metod
        };
        // Banner reklamý göster
        Advertisement.Banner.Show(adUnitId, options);
    }

    public void HideBannerAd()
    {
        // Banner reklamý gizle
        Advertisement.Banner.Hide();
    }


    #region Show Callbacks
    private void BannerHidden()
    {
        Debug.Log("Banner Ad Hidden.");
        // Ýsteðe baðlý: Banner gizlendiðinde oyunun UI'ýný geri getirme veya sesleri açma.
    }

    private void BannerClicked()
    {
        Debug.Log("Banner Ad Clicked.");
        // Ýsteðe baðlý: Oyuncuyu dýþarý yönlendirdiði için oyun seslerini kapatma vb.
    }

    private void BannerShown()
    {
        Debug.Log("Banner Ad Shown.");
        // Ýsteðe baðlý: Banner göründüðünde oyunun UI'ýný ayarlama veya sesleri kýsma.
    }
    #endregion

    #region Load Callbacks
    private void BannerLoadedError(string message)
    {
        Debug.LogError($"Banner Ad Load Error: {message}");
        // Hata durumunda tekrar yüklemeyi deneme veya loglama yapma.
        // Örneðin: Invoke("LoadBannerAd", 5f); // 5 saniye sonra tekrar dene
    }

    private void BannerLoaded()
    {
        Debug.Log("Banner Ad Loaded.");
        // Reklam yüklendiðinde otomatik olarak göster
        //ShowBannerAd();
    }
    #endregion
}