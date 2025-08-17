//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Advertisements;

//public class InterstitialAds : MonoBehaviour , IUnityAdsLoadListener ,IUnityAdsShowListener
//{
//    [SerializeField] private string androidAdUnitId;
//    [SerializeField] private string iosAdUnitId;

//    private string adUnitId;

//    private void Awake()
//    {
//        #if UNITY_IOS
//                adUnitId = iosAdUnitId;
//        #elif UNITY_ANDROID
//                adUnitId = androidAdUnitId;
//        #endif
//    }

//    public void LoadInterstitialAd()
//    {
//        Advertisement.Load(adUnitId, this);
//    }

//    public void ShowInterstitialAd()
//    {
//        Advertisement.Show(adUnitId, this);
//        LoadInterstitialAd();
//    }




//    #region LoadCallbacks
//    public void OnUnityAdsAdLoaded(string placementId)
//    {
//        Debug.Log("Interstitial Ad Loaded");
//    }

//    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
//    {
//        // Hata mesajýný logla
//        Debug.LogError($"Error loading Ad Unit {placementId}: {error.ToString()} - {message}");
//        if (GameManager.instance != null && ResourceManager.Instance != null)
//        {
//            ResourceManager.Instance.GetReweard();
//            //if (GameManager.instance.currentRewarded == RewardedType.Resource)
//            //    GameManager.instance.ReStart();
//        }
//        else
//        {
//            Debug.LogWarning("GameManager veya gameData bulunamadý.");
//        }
//        SceeneManager.instance.LoadScene(0);
//    }


//    #endregion
//    #region ShowCallbacks
//    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
//    {
//        // Hata mesajýný logla
//        Debug.LogError($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
//    }

//    public void OnUnityAdsShowStart(string placementId)    {    }

//    public void OnUnityAdsShowClick(string placementId)    {    }

//    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
//    {
//        if (GameManager.instance != null && ResourceManager.Instance != null)
//        {
//            ResourceManager.Instance.GetReweard();
//            //if (GameManager.instance.currentRewarded == RewardedType.Resource)
//            //    GameManager.instance.ReStart();
//        }
//        else
//        {
//            Debug.LogWarning("GameManager veya gameData bulunamadý.");
//        }
//        SceeneManager.instance.LoadScene(0);
//    }
//    #endregion
//}
