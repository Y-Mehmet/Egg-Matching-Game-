using System;
using com.unity3d.mediation;
using UnityEngine;

public class VideoAdsCode : MonoBehaviour
{
   
    [Header("About LevelPlay Ads")]
    private LevelPlayInterstitialAd interstitialAd; //Instantiate interstitial ads object
    LevelPlayAdFormat[] _legacyAdFormats = new[] { LevelPlayAdFormat.REWARDED }; // because in the new levelplay packages the rewarded ads still use legacy code documentation
#if UNITY_IPHONE
     string _keyApp = "THE KEY IN IRONSOURCE DASHBOARD";
#elif UNITY_ANDROID
    string KeyApp = "233381985";
#else
    string KeyApp = "233381985";
#endif
    [SerializeField]
    private string specificRewardedPlacementId = "hd9474fnh7cx320r";

    private void Awake()
    {
        //IronSource.Agent.setMetaData("is_test_suite", "enable"); 
        
    }
    private void Start()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        IronSource.Agent.validateIntegration();
#endif
        //old Way OF SDK INITIALIZATION
        //IronSource.Agent.init(_keyApp);
        LevelPlay.Init(KeyApp, GetUserId(), _legacyAdFormats); // New Way
    }
    void OnApplicationPause(bool paused)
    {
        IronSource.Agent.onApplicationPause(paused);
    }
    private void OnEnable()
    {
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        //LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        //Reward ADS
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

    }
    private void OnDisable()
    {
        // Unsubscribe from SDK initialization
        LevelPlay.OnInitSuccess -= SdkInitializationCompletedEvent;

        // Reward Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent -= RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent -= RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent -= RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent -= RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent -= RewardedVideoOnAdClickedEvent;

        // Inetrstitial Events
        if (interstitialAd != null)
        {
            // Unsubscribe from all events
            interstitialAd.OnAdLoaded -= InterstitialOnAdLoadedEvent;
            interstitialAd.OnAdLoadFailed -= InterstitialOnAdLoadFailedEvent;
            interstitialAd.OnAdDisplayed -= InterstitialOnAdDisplayedEvent;
            interstitialAd.OnAdDisplayFailed -= InterstitialOnAdDisplayFailedEvent;
            interstitialAd.OnAdClicked -= InterstitialOnAdClickedEvent;
            interstitialAd.OnAdClosed -= InterstitialOnAdClosedEvent;
            interstitialAd.OnAdInfoChanged -= InterstitialOnAdInfoChangedEvent;

            // Destroy the ad
            DestroyInterstitialAd();
        }
    }
    private void SdkInitializationCompletedEvent(LevelPlayConfiguration levelPlayConfiguration)
    {
        //IronSource.Agent.launchTestSuite();
        CreateInterstitialAd();
        LoadInterstitialAd();
    }
    private void SdkInitializationFailedEvent(LevelPlayConfiguration levelPlayConfiguration)
    {

    }

    private void CreateInterstitialAd()
    {
        interstitialAd = new LevelPlayInterstitialAd("773lpvj9bjgo1f5k");

        //interstitial
        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
        //Debug.Log("SDK init");
    }
    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.LoadAd();
        }
        else
        {
            //Debug.LogWarning("Tried to load interstitial before initialization");
        }
    }

    private void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.DestroyAd();
            interstitialAd = null;
        }
    }

    public void AdButton()
    {
        if (IronSource.Agent.isRewardedVideoAvailable() == true)
        {
            ShowReward();
            LoadReward();
        }
        else
        {
            //BECAUSE THERE IS THE CASE WHEN THE PLAYER WANT TO THE ADS BUT THE REAWRAD IN NOT AVAILABLE SO WE MUST NOT MAKE THE PLAYER STUCK IN THE DIFFICULT NUMBER
            ShowInterstitialAd();
        }
    }

    #region Interstitial

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.IsAdReady())
        {
            interstitialAd.ShowAd();
        }
        else
        {
            Debug.LogWarning("Interstitial ad not ready to show");
        }
    }

    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error) { }
    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError) { }
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo) { }

    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        LoadInterstitialAd();
    }
    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo) { }

    #endregion

    #region Reward

    public void LoadReward()
    {
        IronSource.Agent.loadRewardedVideo();
    }

    public void ShowReward()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("IronSource reward not available");
        }
    }

    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
    }

    void RewardedVideoOnAdUnavailable()
    {
    }

    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
    }

    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
    }

    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        //HERE YOU PUT YOUR REWARD FOR THE PLAYER
        Debug.Log("Rewarded video worked");
        ResourceManager.Instance.GetReweard();
    }

    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
    }

    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
    }


    #endregion

    private string GetUserId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}