using System;
using System.Collections.Generic;
using ActionFit_Plugin.Core;
using Singular;
using UnityEngine;

public class Rewards : IDisposable
{
    private readonly string _key;
    private bool _isProcessingCommand; // 명령 처리 상태 추적
    private bool _isRewardEarned = false;
    private static Action _command;
    private static Action _failCommand;
    
    public Rewards(string key)
    {
        MaxSdk.SetVerboseLogging(true);
        MaxSdkBase.InvokeEventsOnUnityMainThread = true;
        _key = key;
    }

    public void Dispose()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnRewardedAdRevenuePaidEvent;
    }

    public void InitializeRewardAds()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(_key);
    }

    public void ShowReward(Action action = null, Action failAction = null)
    {
        ApplicationEventSystem.IsWatchingAd = true;
        _command = action;
        _failCommand = failAction;
        if (!MaxSdk.IsRewardedAdReady(_key))
        {
            CompleteCommandWithFailure();
            return;
        }
        MaxSdk.SetMuted(true);
        MaxSdk.ShowRewardedAd(_key);
    }
    
    private void CompleteCommandWithSuccess()
    {
        _command?.Invoke();
        _command = null;  
        _failCommand = null;
        _isProcessingCommand = false;
        ApplicationEventSystem.IsWatchingAd = false;
    }
    
    private void CompleteCommandWithFailure()
    {
        _failCommand?.Invoke();
        _command = null;  
        _failCommand = null;
        _isProcessingCommand = false;
        ApplicationEventSystem.IsWatchingAd = false;
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("OnRewardedAdLoadedEvent");
    }
    
    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        LoadRewardedAd();
        if (_isProcessingCommand) CompleteCommandWithFailure();
    }
    
    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        RewardLoadFail();
        CompleteCommandWithFailure();
    }
    
    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (_isRewardEarned)
        {
            Debug.Log("보상 지급 처리");
            CompleteCommandWithSuccess();
        }
        else
        {
            Debug.Log("광고 중단 -> 보상 없음");
            CompleteCommandWithFailure();
        }

        _isRewardEarned = false;
        LoadRewardedAd();
    }
    
    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        _isRewardEarned = true;
    }
    
    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double revenue = adInfo.Revenue;
#if ENABLE_SINGULAR_SDK
        SingularAdData data = new SingularAdData("AppLovin_Reward", "USD", revenue);
        SingularSDK.AdRevenue(data);
#endif
    }

    private void RewardLoadFail()
    {
        LoadRewardedAd();
    }
}