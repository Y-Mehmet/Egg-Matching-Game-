public static class AdConfig
{
    public static string AppKey => GetAppKey();
    public static string BannerAdUnitId => GetBannerAdUnitId();
    public static string InterstitalAdUnitId => GetInterstitialAdUnitId();
    public static string RewardedVideoAdUnitId => GetRewardedVideoAdUnitId();

    static string GetAppKey()
    {
#if UNITY_ANDROID
        return "233381985";
#elif UNITY_IPHONE
            return "unexpected_platform";
#else
            return "unexpected_platform";
#endif
    }

    static string GetBannerAdUnitId()
    {
#if UNITY_ANDROID
        return "p0ougyc7mz2nyoyv";
#elif UNITY_IPHONE
            return "unexpected_platform";
#else
            return "unexpected_platform";
#endif
    }
    static string GetInterstitialAdUnitId()
    {
#if UNITY_ANDROID
        return "773lpvj9bjgo1f5k";
#elif UNITY_IPHONE
            return "unexpected_platform";
#else
            return "unexpected_platform";
#endif
    }

    static string GetRewardedVideoAdUnitId()
    {
#if UNITY_ANDROID
        return "hd9474fnh7cx320r";
#elif UNITY_IPHONE
            return "unexpected_platform";
#else
            return "unexpected_platform";
#endif
    }
}
