using UnityEngine;

namespace ActionFit_Plugin.SDK.Max
{
    [CreateAssetMenu(fileName = "@MaxAdsConfig", menuName = "ActionFit/SDK/MaxAdsConfig")]
    public class MaxAdsConfig : ScriptableObject
    {
        [Header("광고가 시작 될 스테이지")] 
        public int defaultAdsLevel;
        
        [Header("광고 쿨타임")] 
        public int commonInterCool;
        
        [Header("앱오픈 쿨타임")] 
        public bool isAppOpen;
        public int appOpenInterCool;
        
        [Header("MaxKey Settings")]
        public string maxKey;
        
        [Header("Android Key Settings")]
        public string androidBannerKey;
        public string androidInterstitialKey;
        public string androidRewardKey;

        [Header("iOS Key Settings")]
        public string iosBannerKey;
        public string iosInterstitialKey;
        public string iosRewardKey;
    
    
        [Header("Google Key Settings")]
        public string androidAppOpenKey;
        public string iosAppOpenKey;

        [Header("Test Device IDs")]
        public TextAsset testDeviceCSV;
        [TextArea(1, 5)] public string[] maxTestDeviceIds;
    }

    public abstract class AdsKey
    {
        public const string AppOpenCooldown = "AppOpen_Inter_Cool";
        public const string CommonCooldown = "Common_Inter_Cool";
    }
}