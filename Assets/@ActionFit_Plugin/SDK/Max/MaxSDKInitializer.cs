#if ENABLE_APPLOVIN_SDK
using System;
using Cysharp.Threading.Tasks;

namespace ActionFit_Plugin.SDK.Max
{
    public struct MaxData
    {
        public string BannerKey;
        public string InterstitialKey;
        public string RewardKey;
        public string AppOpenKey;
        public string[] TestKey;
    }

    public class MaxSDKInitializer
    {
        private string _maxSdkKey;
        private string _adUnitBannerKey;
        private string _adUnitInterstitialKey;
        private string _adUnitRewardKey;
        private string _adUnitAppOpenKey;
        private string[] _maxTestKey;
        private bool _initialize;
    
        private Interstitial Interstitial { get; set; }
        private Rewards Reward { get; set; }
        private Banner Banner { get; set; }
        private AppOpen AppOpen { get; set; }
    
        public async UniTask MaxInit(MaxData keyData)
        {
            await UniTask.Yield();
            MaxSdkBase.InvokeEventsOnUnityMainThread = true;
            _maxTestKey = keyData.TestKey;
            _adUnitBannerKey = keyData.BannerKey;
            _adUnitInterstitialKey = keyData.InterstitialKey;
            _adUnitRewardKey = keyData.RewardKey;
            _adUnitAppOpenKey = keyData.AppOpenKey;
            await MaxInitialize();
        }
        private async UniTask MaxInitialize()
        {
            if(_initialize) return;
            _initialize = true;
            Interstitial = new Interstitial(_adUnitInterstitialKey);
            Reward = new Rewards(_adUnitRewardKey);
            Banner = new Banner(_adUnitBannerKey);
            AppOpen = new AppOpen(_adUnitAppOpenKey);
            var sdkInitCompletionSource = new UniTaskCompletionSource<bool>();
            MaxSdkCallbacks.OnSdkInitializedEvent += success =>
            {
                Interstitial.InitializeInterstitialAds();
                Reward.InitializeRewardAds();
                Banner.InitializeBannerAds();
                AppOpen.InitializeAppOpenAds();
                sdkInitCompletionSource.TrySetResult(true);
            };
            MaxSdk.SetVerboseLogging(false);
            MaxSdk.SetTestDeviceAdvertisingIdentifiers(_maxTestKey);
            MaxSdk.InitializeSdk();
            await sdkInitCompletionSource.Task;
        }
 
        public void ShowInterstitial( Action action = null, Action failAction = null)
        {
            Interstitial.ShowInterstitial(action, failAction);
        }
    
        public void ShowReward(Action action = null, Action failAction = null)
        {
            Reward.ShowReward(action, failAction);
        } 
    
        public void ShowBanner() => Banner.ShowBanner();
        public void HideBanner() => Banner.HideBanner();  
        public void ShowAppOpen() => AppOpen.ShowAppOpenAd();       
    }
}
#endif