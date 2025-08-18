using System.Collections;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.UI; // UI ��eleri i�in gerekli

// Bu s�n�f, reklamlar� otomatik olarak y�kler ve gerekti�inde g�sterir.
public class AdsManager : MonoBehaviour
{
    // Singleton deseni i�in tekil �rnek
    public static AdsManager Instance { get; private set; }

    // Reklam nesneleri
    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayRewardedAd rewardedAd;

    // Y�kleme durumunu g�stermek i�in UI metni
    // Bu metni Unity'de bir Text veya TextMeshPro bile�eni ile ba�layabilirsiniz.


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

    private void OnSdkInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("SDK Init Success. Creating and loading ads.");

        // SDK ba�latma ba�ar�l� oldu�unda reklam nesnelerini olu�tur ve y�kle
        CreateInterstitialAd();
        CreateRewardedAd();
    }

    private void OnSdkInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("SDK Init Failed: " + error.ErrorCode + " - " + error.ErrorMessage);
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
        };
        rewardedAd.OnAdLoadFailed += (error) =>
        {
            Debug.LogError("Rewarded ad failed to load: " + error.ErrorCode + " - " + error.ErrorMessage);
            // Hata durumunda yeniden y�klemeyi dene
            rewardedAd.LoadAd();
        };
        rewardedAd.OnAdClosed += (info) =>
        {
            Debug.Log("Rewarded ad closed. Reloading a new ad...");
            // Reklam kapat�ld���nda yeni bir reklam� hemen y�klemeye ba�la
            rewardedAd.LoadAd();
        };
        rewardedAd.OnAdRewarded += (placement, info) =>
        {
            Debug.Log("Player rewarded!");
            // ResourceManager.Instance.GetReweard(); // Bu sat�r� eski haline getirdim.
            // SceeneManager.instance.LoadScene(0); // Bu sat�r� eski haline getirdim.
        };

        // Reklam� y�klemeye ba�la
        Debug.Log("Loading rewarded ad...");
        rewardedAd.LoadAd();
    }

    /// <summary>
    /// �d�ll� reklam� g�sterir.
    /// Bu metot, oyun i�inde herhangi bir yerden �a�r�labilir (�rn. bir butondan).
    /// </summary>
    public void ShowRewardedAd()
    {
        // Reklam�n haz�r olup olmad���n� kontrol et
        if (rewardedAd != null && rewardedAd.IsAdReady())
        {
            Debug.Log("Rewarded ad is ready. Showing ad.");
            rewardedAd.ShowAd();
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready. Loading ad again.");
            // E�er reklam haz�r de�ilse, yeniden y�klemeyi tetikle.
            rewardedAd.LoadAd();
        }
    }

    // Ge�i� Reklam� (Interstitial) olu�turma ve olaylar�n� dinleme metodu
    private void CreateInterstitialAd()
    {
        interstitialAd = new LevelPlayInterstitialAd(AdConfig.InterstitalAdUnitId);

        // Reklam y�klendi�inde
        interstitialAd.OnAdLoaded += (info) => Debug.Log("Interstitial loaded");
        // Reklam y�klemesi ba�ar�s�z oldu�unda
        interstitialAd.OnAdLoadFailed += (error) => Debug.LogError("Interstitial failed: " + error.ErrorCode + " - " + error.ErrorMessage);
        // Reklam kapat�ld���nda yeni bir reklam� y�kle ve sahneyi de�i�tir
        interstitialAd.OnAdClosed += (info) => { interstitialAd.LoadAd(); SceeneManager.instance.LoadScene(0); };

        // �lk reklam� y�klemeye ba�la
        interstitialAd.LoadAd();
    }

    /// <summary>
    /// Ge�i� reklam�n� g�sterir. Reklam haz�r de�ilse bekler.
    /// </summary>
    public void ShowInterstitialAd()
    {
        // Reklam haz�r m� kontrol et, de�ilse bir Coroutine ba�lat
        if (interstitialAd != null && !interstitialAd.IsAdReady())
        {
            StartCoroutine(WaitForAdReadyCoroutine());
        }
        else
        {
            // Reklam haz�rsa do�rudan g�ster
            if (interstitialAd != null)
            {
                interstitialAd.ShowAd();
            }
            else
            {
                // interstitialAd objesi null ise, bir hata mesaj� ver
                Debug.LogError("Interstitial ad object is null. Did the SDK fail to initialize?");
            }
        }
    }

    private IEnumerator WaitForAdReadyCoroutine()
    {
        Debug.LogWarning("Interstitial not ready. Waiting for ad to load...");


        // Reklam haz�r olana kadar bekle
        while (interstitialAd != null && !interstitialAd.IsAdReady())
        {
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Bekleme bitti�inde, reklam� g�ster
        if (interstitialAd != null && interstitialAd.IsAdReady())
        {
            Debug.Log("Ad is now ready! Showing interstitial.");
            interstitialAd.ShowAd();
        }


    }
}