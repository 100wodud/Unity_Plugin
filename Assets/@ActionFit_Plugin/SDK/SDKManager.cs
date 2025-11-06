using System;
using System.Collections;
using ActionFit_Plugin.Core;
using ActionFit_Plugin.Localize;
using ActionFit_Plugin.SDK.Ads;
using ActionFit_Plugin.SDK.Max;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ActionFit_Plugin.SDK
{
    public static class SDKManager
    {
        private static MaxAdsConfig _maxKey;
#if ENABLE_APPLOVIN_SDK  
        private static MaxSDKInitializer Max { get; set; }
        private static MaxData _maxKeyData;
        public static bool IsInitialized = false;
        public static int DefaultAdsLevel { get; private set; } = 9;
        public static int Common_Inter_Cool { get; private set; } = 60;
        public static int AppOpen_Inter_Cool { get; private set; } = 180;
    
        public static async void Initialized()
        {
            if(IsInitialized) return;
            AsyncOperationHandle<MaxAdsConfig> handle = Addressables.LoadAssetAsync<MaxAdsConfig>("@MaxAdsConfig");
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("[SDKManager] MaxKey Addressables Load Fail!");
                IsInitialized = true;
                return;
            }
            _maxKey = handle.Result;
            Max = new MaxSDKInitializer();
            MaxKeyInitialize();
            
            try
            {
                await Max.MaxInit(_maxKeyData);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SDKManager] MaxSDKInitializer Fail: {e.Message}");
                IsInitialized = true;
            }
            ApplicationEventSystem.OnAppStateForeground += ReturnAppForeground;
            IsInitialized = true;
        }
    
        private static void MaxKeyInitialize()
        {
            DefaultAdsLevel = _maxKey.defaultAdsLevel;
            Common_Inter_Cool = _maxKey.commonInterCool;
            AppOpen_Inter_Cool = _maxKey.appOpenInterCool;
        
            _maxKeyData = new MaxData
            {
                TestKey = _maxKey.maxTestDeviceIds,
#if UNITY_ANDROID || UNITY_EDITOR
                BannerKey = _maxKey.androidBannerKey,
                InterstitialKey = _maxKey.androidInterstitialKey,
                RewardKey = _maxKey.androidRewardKey,
                AppOpenKey = _maxKey.androidAppOpenKey,
#elif UNITY_IOS
                BannerKey = maxKey.iosBannerKey,
                InterstitialKey = maxKey.iosInterstitialKey,
                RewardKey = maxKey.iosRewardKey,
                AppOpenKey = maxKey.iosAppOpenKey,
#endif
            };
        }
    
        #region Ads Manager

        public static async void ShowInterstitial(Action action = null, Action failAction = null, string key = null)
        {
            await UniTask.Delay(300); 
            if(!string.IsNullOrEmpty(key)) AdExtension.UpdateLastCooldownTime(key);
            
            try
            {
                Max.ShowInterstitial(action, failAction);
            }
            catch (Exception e)
            {
                failAction?.Invoke();
                ApplicationEventSystem.IsWatchingAd = false;
                Debug.Log("Error Interstitial Ads: " + e.Message);
            }
        }
        
        public static async void ShowReward( Action action = null, Action failAction = null)
        {
            await UniTask.Delay(300);
            try
            {
                Max.ShowReward(action, failAction);
            }
            catch (Exception e)
            {
                failAction?.Invoke();
                ApplicationEventSystem.IsWatchingAd = false;
                Debug.Log("Error Reward Ads: " + e.Message);
            }
        }
    
        public static void ShowBanner() => Max.ShowBanner();
        public static void HideBanner() => Max.HideBanner();
        public static void ShowAppOpenAd() => Max.ShowAppOpen();

        public static bool InterstitialCondition(string type, bool force = false)
        {
            if (force) return true;
        
            switch (type)
            {
                case AdsKey.AppOpenCooldown:
                    if (PlayerData.ClearLevel >= DefaultAdsLevel && AdExtension.InterCondition(AdsKey.AppOpenCooldown, AppOpen_Inter_Cool)) return true;
                    break;
            
                case AdsKey.CommonCooldown:
                    if (PlayerData.ClearLevel >= DefaultAdsLevel && AdExtension.InterCondition(AdsKey.CommonCooldown, Common_Inter_Cool)) return true;
                    break;
                default:
                    return false;
            }
            return false;
        }

        private static void ReturnAppForeground()
        {
            if(!_maxKey.isAppOpen) return;
        
            if (InterstitialCondition(AdsKey.AppOpenCooldown))
            {
                Max.ShowAppOpen();
                AdExtension.UpdateLastCooldownTime(AdsKey.AppOpenCooldown);
            }
        }

        #endregion
#endif
    }
}
