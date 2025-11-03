using System;
using System.Collections.Generic;
using Singular;
using UnityEngine;

public class Banner
{
    private readonly string _key;
    private bool _showBanner;
    private bool _isAlreadyDestroy;
    private readonly object _lockObject = new();
 
    public Banner(string key)
    {
        MaxSdkBase.InvokeEventsOnUnityMainThread = true;
        _key = key;
    }

    public void InitializeBannerAds()
    {
        lock (_lockObject)
        {
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnBannerAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            MaxSdk.CreateBanner(_key, MaxSdkBase.BannerPosition.BottomCenter);
        }
    }
    
    public void ShowBanner()
    {
        lock (_lockObject)
        {
            if(_showBanner) return;
            MaxSdk.ShowBanner(_key);
            _showBanner = true;
        }
    }

    public void HideBanner()
    {
        lock (_lockObject)
        {
            MaxSdk.HideBanner(_key);
            _showBanner = false;
        }
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double revenue = adInfo.Revenue;
#if ENABLE_SINGULAR_SDK
        SingularAdData data = new SingularAdData("AppLovin_Banner", "USD", revenue);
        SingularSDK.AdRevenue(data);
#endif
    }
}