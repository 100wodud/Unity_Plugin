using System;
using System.Collections;
using ActionFit_Plugin.Core;
using ActionFit_Plugin.SDK.Ads;
using ActionFit_Plugin.SDK.Max;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ActionFit_Plugin.SDK
{
    public class SDKManager : MonoBehaviour
    {
        [SerializeField] private MaxAdsConfig maxKey;
#if ENABLE_APPLOVIN_SDK  
        public static SDKManager Instance { get; private set; }
        private MaxSDKInitializer Max { get; set; }
        private MaxData _maxKeyData;
        private bool _initialize;
        private bool _initFirebase;
        public static int DefaultAdsLevel { get; private set; } = 9;
        public static int Common_Inter_Cool { get; private set; } = 60;
        public static int AppOpen_Inter_Cool { get; private set; } = 180;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    
        public async UniTask Initialized()
        {
            if(_initialize) return;
            _initialize = true;
            Max = new MaxSDKInitializer();
            MaxKeyInitialize();
            await Max.MaxInit(_maxKeyData);
            ApplicationEventSystem.OnAppStateForeground += ReturnAppForeground;
        }
    
        private void MaxKeyInitialize()
        {
            DefaultAdsLevel = maxKey.defaultAdsLevel;
            Common_Inter_Cool = maxKey.commonInterCool;
            AppOpen_Inter_Cool = maxKey.appOpenInterCool;
        
            _maxKeyData = new MaxData
            {
                Key = maxKey.maxKey,   
                TestKey = maxKey.maxTestDeviceIds,
#if UNITY_ANDROID || UNITY_EDITOR
                BannerKey = maxKey.androidBannerKey,
                InterstitialKey = maxKey.androidInterstitialKey,
                RewardKey = maxKey.androidRewardKey,
                AppOpenKey = maxKey.androidAppOpenKey,
#elif UNITY_IOS
                BannerKey = maxKey.iosBannerKey,
                InterstitialKey = maxKey.iosInterstitialKey,
                RewardKey = maxKey.iosRewardKey,
                AppOpenKey = maxKey.iosAppOpenKey,
#endif
            };
        }
    
        #region Ads Manager

        public async void ShowIn(string key, Action action = null, Action failAction = null)
        {
            await UniTask.Delay(300); 
            AdExtension.UpdateLastCooldownTime(key);
            Max.ShowInterstitial(action, failAction);
        }
    
        public void ShowRe(Action action = null, Action failAction = null) => StartCoroutine(RewardShow(action, failAction));
        
        private IEnumerator RewardShow( Action action = null, Action failAction = null)
        {
            yield return new WaitForSecondsRealtime(0.3f);
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
    
        public void ShowBanner() => Max.ShowBanner();
        public void HideBanner() => Max.HideBanner();
        public void LoadReward() => Max.LoadReward();
        public void ShowAppOpenAd() => Max.ShowAppOpen();

        public bool InterstitialCondition(string type, bool force = false)
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

        private void ReturnAppForeground()
        {
            if(!maxKey.isAppOpen) return;
        
            if (InterstitialCondition(AdsKey.AppOpenCooldown))
            {
                Max.ShowAppOpen();
                AdExtension.UpdateLastCooldownTime(AdsKey.AppOpenCooldown);
            }
        }

        #endregion
    }
#endif
}
