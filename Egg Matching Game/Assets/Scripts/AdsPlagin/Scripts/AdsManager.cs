using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Bu sat�r� ekleyin!

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

   
       

        // Ba�lang��ta reklamlar� y�kle (Awake'te y�klemek yerine, ihtiyaca g�re LoadBannerAd() i�ini d�zenleyebilirsiniz)
        // bannerAds.LoadBannerAd(); // Bu sat�r� �imdilik yoruma al�yoruz, sahneye g�re y�netece�iz.
        interstitialAds.LoadInterstitialAd();
        rewardedAds.LoadRewardedAd();
    }

    private void OnDestroy()
    {
    }

   
}