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

    // Reklam y�klemesi i�in zaman a��m� s�resi (saniye)
    private const float AdLoadTimeout = 10.0f;

    // �d�ll� reklam�n haz�r olup olmad���n� kontrol etme
    public bool IsRewardedAdReady => rewardedAd != null && rewardedAd.IsAdReady();

    // Ge�i� reklam�n�n haz�r olup olmad���n� kontrol etme
    public bool IsInterstitialAdReady => interstitialAd != null && interstitialAd.IsAdReady();
    


    private void Awake()
    {
        // Singleton ayarlar�
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
        // SDK ba�latma olaylar�n� dinle
        LevelPlay.OnInitSuccess += OnSdkInitSuccess;
        LevelPlay.OnInitFailed += OnSdkInitFailed;

        // SDK'y� ba�lat
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

        // SDK ba�latma ba�ar�l� oldu�unda reklam nesnelerini olu�tur ve y�kle
        CreateInterstitialAd();
        CreateRewardedAd();
        // BANNER REKLAMI OLU�TURMA VE Y�KLEME
        CreateBannerAd();
    }

    private void OnSdkInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("SDK Init Failed: " + error.ErrorCode + " - " + error.ErrorMessage);
        Debug.Log("SDK Init Failed.");
    }

    // �d�ll� Reklam (Rewarded Ad) olu�turma ve olaylar�n� dinleme metodu
    private void CreateRewardedAd()
    {
        // Reklam kimli�ini AdConfig s�n�f�ndan al�yoruz.
        rewardedAd = new LevelPlayRewardedAd(AdConfig.RewardedVideoAdUnitId);

        // Olay dinleyicilerini tan�mla
        rewardedAd.OnAdLoaded += (info) =>
        {
            Debug.Log("Rewarded ad loaded successfully.");
            Debug.Log("Rewarded ad is ready.");
        };
        rewardedAd.OnAdLoadFailed += (error) =>
        {
            Debug.LogError("Rewarded ad failed to load: " + error.ErrorCode + " - " + error.ErrorMessage);
            Debug.Log("Rewarded ad failed to load. Retrying...");
            // Hata durumunda yeniden y�klemeyi dene
            rewardedAd.LoadAd();
        };
        rewardedAd.OnAdClosed += (info) =>
        {
            Debug.Log("Rewarded ad closed. Reloading a new ad...");
            Debug.Log("Ad closed. Reloading rewarded ad.");
            // Reklam kapat�ld���nda yeni bir reklam� hemen y�klemeye ba�la
            rewardedAd.LoadAd();
        };
        rewardedAd.OnAdRewarded += (placement, info) =>
        {
            Debug.Log("Player rewarded!");
            Debug.Log("Player rewarded!");
            // �d�l verme ve sahne de�i�tirme mant���
            ResourceManager.Instance.GetReweard();
            SceeneManager.instance.LoadScene(0);
        };

        // Reklam� y�klemeye ba�la
        Debug.Log("Loading rewarded ad...");
        Debug.Log("Loading rewarded ad...");
        rewardedAd.LoadAd();
    }

    /// <summary>
    /// �d�ll� reklam� g�sterir.
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

    // Ge�i� Reklam� (Interstitial) olu�turma ve olaylar�n� dinleme metodu
    private void CreateInterstitialAd()
    {
        interstitialAd = new LevelPlayInterstitialAd(AdConfig.InterstitalAdUnitId);

        // Reklam y�klendi�inde
        interstitialAd.OnAdLoaded += (info) => {
            Debug.Log("Interstitial loaded");
            Debug.Log("Interstitial is ready.");
        };
        // Reklam y�klemesi ba�ar�s�z oldu�unda
        interstitialAd.OnAdLoadFailed += (error) => {
            Debug.LogError("Interstitial failed: " + error.ErrorCode + " - " + error.ErrorMessage);
            Debug.Log("Interstitial failed to load. Retrying...");
            interstitialAd.LoadAd();
        };
        // Reklam kapat�ld���nda yeni bir reklam� y�kle ve sahneyi de�i�tir
        interstitialAd.OnAdClosed += (info) => {
            interstitialAd.LoadAd();
            Debug.Log("Interstitial closed. Reloading.");
            ResourceManager.Instance.GetReweard();
            SceeneManager.instance.LoadScene(0);
        };

        // �lk reklam� y�klemeye ba�la
        interstitialAd.LoadAd();
        Debug.Log("Loading interstitial...");
    }

    /// <summary>
    /// Ge�i� reklam�n� g�sterir. Reklam haz�r de�ilse zaman a��m� ile bekler.
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

    // Reklam�n zaman a��m� ile haz�r olmas�n� bekleyen Coroutine
    private IEnumerator WaitForAdReadyWithTimeout(float timeout)
    {
        float startTime = Time.time;

        while (!IsInterstitialAdReady && Time.time - startTime < timeout)
        {
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Bekleme bitti�inde veya reklam haz�r oldu�unda
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

    // BANNER REKLAM KODU BURAYA EKLEN�YOR
    private void CreateBannerAd()
    {

        bannerAd = new LevelPlayBannerAd(AdConfig.BannerAdUnitId);

        // Banner reklam y�klendi�inde
        bannerAd.OnAdLoaded += (info) =>
        {
            Debug.Log("Banner ad loaded successfully.");
            // Reklam y�klendi�inde otomatik olarak g�ster
           
        };

        // Banner reklam y�klemesi ba�ar�s�z oldu�unda
        bannerAd.OnAdLoadFailed += (error) =>
        {
            Debug.LogError("Banner ad failed to load: " + error.ErrorCode + " - " + error.ErrorMessage);
        };

        // Banner reklam� y�klemeye ba�la
        Debug.Log("Loading banner ad...");
        bannerAd.LoadAd();
        HideBannerAd();
    }

    /// <summary>
    /// Banner reklam�n� gizler.
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
    /// Banner reklam�n� g�sterir.
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