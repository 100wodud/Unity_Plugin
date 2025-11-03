using System;
using System.Collections.Generic;
using ActionFit_Plugin.Core;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using Singular;
using UnityEngine;

public class AppOpen : IDisposable
{
    private readonly string _key;
    private AppOpenAd _appOpenAd;

    public AppOpen(string key)
    {
        _key = key;
    }
    
    public void InitializeAppOpenAds()
    {
        Debug.Log("Initializing MobileAds...");
        MobileAds.Initialize(_ =>
        {
            LoadAppOpenAd();
        });
    }
    
    public void Dispose()
    {
        // appOpenAd?.Destroy();
        // appOpenAd = null;
    }
    
    public async void ShowAppOpenAd()
    {
        ApplicationEventSystem.IsWatchingAd = true;
        
        await UniTask.Delay(500);
        if (_appOpenAd != null && _appOpenAd.CanShowAd())
        {
            _appOpenAd.Show();
        }
        else
        {
            ApplicationEventSystem.IsWatchingAd = false;
            Debug.LogWarning("AppOpenAd is not loaded yet.");
        }
    }

    public void LoadAppOpenAd()
    {
        if (_appOpenAd is not null)
        {
            _appOpenAd.Destroy();
            _appOpenAd = null;
        }
                
        AdRequest adRequest = new AdRequest();
                
        AppOpenAd.Load(_key, adRequest, (ad, e) =>
        {
            if (e is not null || ad is null)
            {
                Debug.LogError("app open ad failed to load an ad " + "with error : " + e);
                return;
            }
            _appOpenAd = ad;
            RegisterEventHandlers(_appOpenAd);
        });
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += SendSingularRevenue;
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            ApplicationEventSystem.IsWatchingAd = false;
            LoadAppOpenAd();
            Debug.Log("App open ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            ApplicationEventSystem.IsWatchingAd = false;
            LoadAppOpenAd();
            Debug.LogError("App open ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    private void SendSingularRevenue(AdValue adValue)
    {
        LoadAppOpenAd();
    }
}
