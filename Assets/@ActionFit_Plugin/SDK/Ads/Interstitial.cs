#if ENABLE_APPLOVIN_SDK
using System;
using ActionFit_Plugin.Core;
using UnityEngine;
#if ENABLE_SINGULAR_SDK
using Singular;
#endif

public class Interstitial : IDisposable
{
    private readonly string _key;
    private bool _isLoadingInterstitial = false;
    private static Action _command;
    private static Action _failCommand;
    
    public Interstitial(string key)
    {
        _key = key;
    }
    
    public void Dispose()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnInterstitialRevenuePaidEvent;
    }


    public void InitializeInterstitialAds()
    {
        MaxSdkBase.InvokeEventsOnUnityMainThread = true;
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialDisplayFailedEvent;
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        if (_isLoadingInterstitial) return; 
        _isLoadingInterstitial = true;
        MaxSdk.LoadInterstitial(_key);
    }

    public void ShowInterstitial( Action action = null, Action failAction = null )
    {
        ApplicationEventSystem.IsWatchingAd = true;
        _command = action;
        _failCommand = failAction;
        if (!MaxSdk.IsInterstitialReady(_key))
        {
            FailCommand();
            return;
        }
        MaxSdk.SetMuted(true);
        MaxSdk.ShowInterstitial(_key);
    }
    
    private void CompleteCommand()
    {
        _command?.Invoke();
        _command = null;
        _failCommand = null;
        _isLoadingInterstitial = false;
        ApplicationEventSystem.IsWatchingAd = false;
        LoadInterstitial();
    }

    private void FailCommand()
    {
        _failCommand?.Invoke();
        _command = null;
        _failCommand = null;
        _isLoadingInterstitial = false;
        ApplicationEventSystem.IsWatchingAd = false;
        LoadInterstitial();
    }
    
    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        string countyCode = MaxSdk.GetSdkConfiguration().CountryCode;
        Debug.Log($"OnInterstitialLoadedEvent: {countyCode}");
        _isLoadingInterstitial = false;
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log($"OnInterstitialFailedEvent");
        FailCommand();
    }

    private void OnInterstitialDisplayFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2, MaxSdkBase.AdInfo arg3)
    {
        Debug.Log($"OnInterstitialDisplayFailedEvent");
        FailCommand();
    }
    
    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log($"OnInterstitialDismissedEvent");
        CompleteCommand();
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double revenue = adInfo.Revenue;
#if ENABLE_SINGULAR_SDK
        SingularAdData data = new SingularAdData("AppLovin_Interstitial", "USD", revenue);
        SingularSDK.AdRevenue(data);
#endif
    }
}
#endif