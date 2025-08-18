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

        // Banner'�n konumunu ayarla (�rne�in ekran�n alt ortas�)
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);

        // Oyun ba�lad���nda veya bu script aktif oldu�unda banner reklam� y�kle
        LoadBannerAd();
    }

    public void LoadBannerAd()
    {
        // Banner reklam y�kleme se�eneklerini belirle
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = BannerLoaded, // Reklam y�klendi�inde �a�r�lacak metod
            errorCallback = BannerLoadedError // Y�kleme hatas� olu�tu�unda �a�r�lacak metod
        };

        // Banner reklam� belirtilen reklam birimi kimli�i ile y�kle
        Advertisement.Banner.Load(adUnitId, options);
    }

    public void ShowBannerAd()
    {
        // Banner reklam g�sterme se�eneklerini belirle
        BannerOptions options = new BannerOptions
        {
            showCallback = BannerShown, // Reklam g�sterildi�inde �a�r�lacak metod
            clickCallback = BannerClicked, // Reklama t�kland���nda �a�r�lacak metod
            hideCallback = BannerHidden // Reklam gizlendi�inde �a�r�lacak metod
        };
        // Banner reklam� g�ster
        Advertisement.Banner.Show(adUnitId, options);
    }

    public void HideBannerAd()
    {
        // Banner reklam� gizle
        Advertisement.Banner.Hide();
    }


    #region Show Callbacks
    private void BannerHidden()
    {
        Debug.Log("Banner Ad Hidden.");
        // �ste�e ba�l�: Banner gizlendi�inde oyunun UI'�n� geri getirme veya sesleri a�ma.
    }

    private void BannerClicked()
    {
        Debug.Log("Banner Ad Clicked.");
        // �ste�e ba�l�: Oyuncuyu d��ar� y�nlendirdi�i i�in oyun seslerini kapatma vb.
    }

    private void BannerShown()
    {
        Debug.Log("Banner Ad Shown.");
        // �ste�e ba�l�: Banner g�r�nd���nde oyunun UI'�n� ayarlama veya sesleri k�sma.
    }
    #endregion

    #region Load Callbacks
    private void BannerLoadedError(string message)
    {
        Debug.LogError($"Banner Ad Load Error: {message}");
        // Hata durumunda tekrar y�klemeyi deneme veya loglama yapma.
        // �rne�in: Invoke("LoadBannerAd", 5f); // 5 saniye sonra tekrar dene
    }

    private void BannerLoaded()
    {
        Debug.Log("Banner Ad Loaded.");
        // Reklam y�klendi�inde otomatik olarak g�ster
        //ShowBannerAd();
    }
    #endregion
}