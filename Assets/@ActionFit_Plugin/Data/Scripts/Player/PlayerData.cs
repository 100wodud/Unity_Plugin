using System;
using ActionFit_Plugin.Data.Scripts;
using JetBrains.Annotations;
using UnityEngine;

public static class PlayerData
{
    public static bool IsInitialized { get; private set; }
    private static PlayerDataSave _save;

    public static void Init()
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

    public static int ClearLevel
    {
        get => _save.clearLevel;
        set => SetAndSave(ref _save.clearLevel, value, v => _save.clearLevel = v, PlayerDataEvent.InvokeLevel);
    }
}