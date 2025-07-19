using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Bu satýrý ekleyin!

public class AdsManager : MonoBehaviour
{
    public InitializeAds initializeAds;
    public BannerAds bannerAds;
    public InterstitialAds interstitialAds;
    public RewardedAds rewardedAds;

    public static AdsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

   
       

        // Baþlangýçta reklamlarý yükle (Awake'te yüklemek yerine, ihtiyaca göre LoadBannerAd() içini düzenleyebilirsiniz)
        // bannerAds.LoadBannerAd(); // Bu satýrý þimdilik yoruma alýyoruz, sahneye göre yöneteceðiz.
        interstitialAds.LoadInterstitialAd();
        rewardedAds.LoadRewardedAd();
    }

    private void OnDestroy()
    {
    }

   
}