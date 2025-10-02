using System.Collections;

using Unity.Services.LevelPlay;
using Unity.VisualScripting.InputSystem;
using UnityEngine;





public class AdsManager : MonoBehaviour
{
  
    public static AdsManager Instance { get; private set; }

   
    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayRewardedAd rewardedAd;


   
    private LevelPlayBannerAd bannerAd;

    // Reklam yüklemesi için zaman aþýmý süresi (saniye)
    private const float AdLoadTimeout = 10.0f;

    // Ödüllü reklamýn hazýr olup olmadýðýný kontrol etme
    public bool IsRewardedAdReady => rewardedAd != null && rewardedAd.IsAdReady();

    // Geçiþ reklamýnýn hazýr olup olmadýðýný kontrol etme
    public bool IsInterstitialAdReady => interstitialAd != null && interstitialAd.IsAdReady();
    


    private void Awake()
    {
        // Singleton ayarlarý
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // SDK baþlatma olaylarýný dinle
        LevelPlay.OnInitSuccess += OnSdkInitSuccess;
        LevelPlay.OnInitFailed += OnSdkInitFailed;

        // SDK'yý baþlat
        LevelPlay.Init(AdConfig.AppKey);
    }
    private void OnEnable()
    {
        HideBannerAd();
    }
    private void OnSdkInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("SDK Init Success. Creating and loading ads.");
        Debug.Log("SDK Init Success.");

        // SDK baþlatma baþarýlý olduðunda reklam nesnelerini oluþtur ve yükle
        CreateInterstitialAd();
        CreateRewardedAd();
        // BANNER REKLAMI OLUÞTURMA VE YÜKLEME
        CreateBannerAd();
    }

    private void OnSdkInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("SDK Init Failed: " + error.ErrorCode + " - " + error.ErrorMessage);
        Debug.Log("SDK Init Failed.");
    }

    // Ödüllü Reklam (Rewarded Ad) oluþturma ve olaylarýný dinleme metodu
    private void CreateRewardedAd()
    {
        // Reklam kimliðini AdConfig sýnýfýndan alýyoruz.
        rewardedAd = new LevelPlayRewardedAd(AdConfig.RewardedVideoAdUnitId);

        // Olay dinleyicilerini tanýmla
        rewardedAd.OnAdLoaded += (info) =>
        {
            Debug.Log("Rewarded ad loaded successfully.");
            Debug.Log("Rewarded ad is ready.");
        };
        rewardedAd.OnAdLoadFailed += (error) =>
        {
            Debug.LogError("Rewarded ad failed to load: " + error.ErrorCode + " - " + error.ErrorMessage);
            Debug.Log("Rewarded ad failed to load. Retrying...");
            // Hata durumunda yeniden yüklemeyi dene
            rewardedAd.LoadAd();
        };
        rewardedAd.OnAdClosed += (info) =>
        {
            Debug.Log("Rewarded ad closed. Reloading a new ad...");
            Debug.Log("Ad closed. Reloading rewarded ad.");
            // Reklam kapatýldýðýnda yeni bir reklamý hemen yüklemeye baþla
            rewardedAd.LoadAd();
        };
        rewardedAd.OnAdRewarded += (placement, info) =>
        {
            Debug.Log("Player rewarded!");
            Debug.Log("Player rewarded!");
            // Ödül verme ve sahne deðiþtirme mantýðý
            ResourceManager.Instance.GetReweard();
            SceeneManager.instance.LoadScene(0);
        };

        // Reklamý yüklemeye baþla
        Debug.Log("Loading rewarded ad...");
        Debug.Log("Loading rewarded ad...");
        rewardedAd.LoadAd();
    }

    /// <summary>
    /// Ödüllü reklamý gösterir.
    /// </summary>
    public void ShowRewardedAd()
    {
        if (IsRewardedAdReady)
        {
            Debug.Log("Rewarded ad is ready. Showing ad.");
            rewardedAd.ShowAd();
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready. Loading ad again.");
            Debug.Log("Ad not ready. Retrying.");
            rewardedAd.LoadAd();
        }
    }

    // Geçiþ Reklamý (Interstitial) oluþturma ve olaylarýný dinleme metodu
    private void CreateInterstitialAd()
    {
        interstitialAd = new LevelPlayInterstitialAd(AdConfig.InterstitalAdUnitId);

        // Reklam yüklendiðinde
        interstitialAd.OnAdLoaded += (info) => {
            Debug.Log("Interstitial loaded");
            Debug.Log("Interstitial is ready.");
        };
        // Reklam yüklemesi baþarýsýz olduðunda
        interstitialAd.OnAdLoadFailed += (error) => {
            Debug.LogError("Interstitial failed: " + error.ErrorCode + " - " + error.ErrorMessage);
            Debug.Log("Interstitial failed to load. Retrying...");
            interstitialAd.LoadAd();
        };
        // Reklam kapatýldýðýnda yeni bir reklamý yükle ve sahneyi deðiþtir
        interstitialAd.OnAdClosed += (info) => {
            interstitialAd.LoadAd();
            Debug.Log("Interstitial closed. Reloading.");
            ResourceManager.Instance.GetReweard();
            SceeneManager.instance.LoadScene(0);
        };

        // Ýlk reklamý yüklemeye baþla
        interstitialAd.LoadAd();
        Debug.Log("Loading interstitial...");
    }

    /// <summary>
    /// Geçiþ reklamýný gösterir. Reklam hazýr deðilse zaman aþýmý ile bekler.
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (IsInterstitialAdReady)
        {
            interstitialAd.ShowAd();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready. Starting wait coroutine.");
            Debug.Log("Ad not ready. Waiting for " + AdLoadTimeout + "s.");
            StartCoroutine(WaitForAdReadyWithTimeout(AdLoadTimeout));
        }
    }

    // Reklamýn zaman aþýmý ile hazýr olmasýný bekleyen Coroutine
    private IEnumerator WaitForAdReadyWithTimeout(float timeout)
    {
        float startTime = Time.time;

        while (!IsInterstitialAdReady && Time.time - startTime < timeout)
        {
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Bekleme bittiðinde veya reklam hazýr olduðunda
        if (IsInterstitialAdReady)
        {
            Debug.Log("Ad is now ready! Showing interstitial.");
            Debug.Log("Ad is ready. Showing ad.");
            interstitialAd.ShowAd();
        }
        else
        {
            Debug.LogWarning("Interstitial ad did not become ready within the timeout period.");
            Debug.Log("Ad failed to load within timeout.");
        }
    }

    // BANNER REKLAM KODU BURAYA EKLENÝYOR
    private void CreateBannerAd()
    {

        bannerAd = new LevelPlayBannerAd(AdConfig.BannerAdUnitId);

        // Banner reklam yüklendiðinde
        bannerAd.OnAdLoaded += (info) =>
        {
            Debug.Log("Banner ad loaded successfully.");
            // Reklam yüklendiðinde otomatik olarak göster
           
        };

        // Banner reklam yüklemesi baþarýsýz olduðunda
        bannerAd.OnAdLoadFailed += (error) =>
        {
            Debug.LogError("Banner ad failed to load: " + error.ErrorCode + " - " + error.ErrorMessage);
        };

        // Banner reklamý yüklemeye baþla
        Debug.Log("Loading banner ad...");
        bannerAd.LoadAd();
        HideBannerAd();
    }

    /// <summary>
    /// Banner reklamýný gizler.
    /// </summary>
    public void HideBannerAd()
    {
        if (bannerAd != null)
        {
            bannerAd.HideAd();
            Debug.Log("Banner ad is hidden.");
        }
    }

    /// <summary>
    /// Banner reklamýný gösterir.
    /// </summary>
    public void ShowBannerAd()
    {
        if (bannerAd != null)
        {
            bannerAd.ShowAd();
            Debug.Log("Banner ad is shown.");
        }
    }
}