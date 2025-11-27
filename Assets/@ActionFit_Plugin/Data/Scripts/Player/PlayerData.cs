using System;
using ActionFit_Plugin.Data.Scripts;
using JetBrains.Annotations;
using UnityEngine;

public static class PlayerData
{
    public static bool IsInitialized { get; private set; }
    private static PlayerDataSave _save;

    public static void Initialized()
    {
        SaveController.Init();
        _save = SaveController.GetSaveObject<PlayerDataSave>("PlayerDataSave");
        IsInitialized = true;
        Debug.Log("[PlayerData] Initialized");
    }

    private static void SetAndSave<T>([NotNull] ref T field, T value, Action<T> saveSetter, Action eventCallback = null)
    {
        if (field == null) throw new ArgumentNullException(nameof(field));
        field = value;
        saveSetter(value);
        SaveController.MarkAsSaveIsRequired();
        eventCallback?.Invoke();
    }
    public static bool AdsRemove
    {
        get => _save.adsRemove;
        set => SetAndSave(ref _save.adsRemove, value, v => _save.adsRemove = v);
    }

    public static int ClearLevel
    {
        get => _save.clearLevel;
        set => SetAndSave(ref _save.clearLevel, value, v => _save.clearLevel = v, PlayerDataEvent.InvokeLevel);
    }
    
    #region Heart
    
    public const int MaxHeart = 5;
    public const int HeartInterval = 15;
    
    public static int Heart
    {
        get => _save.heart;
        set => SetAndSave(ref _save.heart, value, v => _save.heart = v, PlayerDataEvent.InvokeHeart);
    }

    public static long LastHeartUsedTime
    {
        get => _save.lastHeartUsedTime;
        set => SetAndSave(ref _save.lastHeartUsedTime, value, v => _save.lastHeartUsedTime = v);
    }

    public static long HeartUnlimitedTime
    {
        get => _save.heartUnlimitedTime;
        set => SetAndSave(ref _save.heartUnlimitedTime, value, v => _save.heartUnlimitedTime = v, PlayerDataEvent.InvokeUnlimitedHeart);
    }

    #endregion
}