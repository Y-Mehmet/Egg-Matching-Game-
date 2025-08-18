using System.Collections;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.UI; // UI öðeleri için gerekli

// Bu sýnýf, reklamlarý otomatik olarak yükler ve gerektiðinde gösterir.
public class AdsManager : MonoBehaviour
{
    // Singleton deseni için tekil örnek
    public static AdsManager Instance { get; private set; }

    // Reklam nesneleri
    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayRewardedAd rewardedAd;

    // Yükleme durumunu göstermek için UI metni
    // Bu metni Unity'de bir Text veya TextMeshPro bileþeni ile baðlayabilirsiniz.


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

    private void OnSdkInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("SDK Init Success. Creating and loading ads.");

        // SDK baþlatma baþarýlý olduðunda reklam nesnelerini oluþtur ve yükle
        CreateInterstitialAd();
        CreateRewardedAd();
    }

    private void OnSdkInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("SDK Init Failed: " + error.ErrorCode + " - " + error.ErrorMessage);
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
        };
        rewardedAd.OnAdLoadFailed += (error) =>
        {
            Debug.LogError("Rewarded ad failed to load: " + error.ErrorCode + " - " + error.ErrorMessage);
            // Hata durumunda yeniden yüklemeyi dene
            rewardedAd.LoadAd();
        };
        rewardedAd.OnAdClosed += (info) =>
        {
            Debug.Log("Rewarded ad closed. Reloading a new ad...");
            // Reklam kapatýldýðýnda yeni bir reklamý hemen yüklemeye baþla
            rewardedAd.LoadAd();
        };
        rewardedAd.OnAdRewarded += (placement, info) =>
        {
            Debug.Log("Player rewarded!");
            // ResourceManager.Instance.GetReweard(); // Bu satýrý eski haline getirdim.
            // SceeneManager.instance.LoadScene(0); // Bu satýrý eski haline getirdim.
        };

        // Reklamý yüklemeye baþla
        Debug.Log("Loading rewarded ad...");
        rewardedAd.LoadAd();
    }

    /// <summary>
    /// Ödüllü reklamý gösterir.
    /// Bu metot, oyun içinde herhangi bir yerden çaðrýlabilir (örn. bir butondan).
    /// </summary>
    public void ShowRewardedAd()
    {
        // Reklamýn hazýr olup olmadýðýný kontrol et
        if (rewardedAd != null && rewardedAd.IsAdReady())
        {
            Debug.Log("Rewarded ad is ready. Showing ad.");
            rewardedAd.ShowAd();
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready. Loading ad again.");
            // Eðer reklam hazýr deðilse, yeniden yüklemeyi tetikle.
            rewardedAd.LoadAd();
        }
    }

    // Geçiþ Reklamý (Interstitial) oluþturma ve olaylarýný dinleme metodu
    private void CreateInterstitialAd()
    {
        interstitialAd = new LevelPlayInterstitialAd(AdConfig.InterstitalAdUnitId);

        // Reklam yüklendiðinde
        interstitialAd.OnAdLoaded += (info) => Debug.Log("Interstitial loaded");
        // Reklam yüklemesi baþarýsýz olduðunda
        interstitialAd.OnAdLoadFailed += (error) => Debug.LogError("Interstitial failed: " + error.ErrorCode + " - " + error.ErrorMessage);
        // Reklam kapatýldýðýnda yeni bir reklamý yükle ve sahneyi deðiþtir
        interstitialAd.OnAdClosed += (info) => { interstitialAd.LoadAd(); SceeneManager.instance.LoadScene(0); };

        // Ýlk reklamý yüklemeye baþla
        interstitialAd.LoadAd();
    }

    /// <summary>
    /// Geçiþ reklamýný gösterir. Reklam hazýr deðilse bekler.
    /// </summary>
    public void ShowInterstitialAd()
    {
        // Reklam hazýr mý kontrol et, deðilse bir Coroutine baþlat
        if (interstitialAd != null && !interstitialAd.IsAdReady())
        {
            StartCoroutine(WaitForAdReadyCoroutine());
        }
        else
        {
            // Reklam hazýrsa doðrudan göster
            if (interstitialAd != null)
            {
                interstitialAd.ShowAd();
            }
            else
            {
                // interstitialAd objesi null ise, bir hata mesajý ver
                Debug.LogError("Interstitial ad object is null. Did the SDK fail to initialize?");
            }
        }
    }

    private IEnumerator WaitForAdReadyCoroutine()
    {
        Debug.LogWarning("Interstitial not ready. Waiting for ad to load...");


        // Reklam hazýr olana kadar bekle
        while (interstitialAd != null && !interstitialAd.IsAdReady())
        {
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Bekleme bittiðinde, reklamý göster
        if (interstitialAd != null && interstitialAd.IsAdReady())
        {
            Debug.Log("Ad is now ready! Showing interstitial.");
            interstitialAd.ShowAd();
        }


    }
}