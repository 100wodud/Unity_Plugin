using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ActionFit_Plugin.Localize
{
    public static class LocalizeInitializer
    {
        public static bool IsInitialized;
        private static LocalizeProvider _provider;

        public static async void Initialized()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            AsyncOperationHandle<LocalizeProvider> handle = Addressables.LoadAssetAsync<LocalizeProvider>("LocalizeProvider");
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("[Localized] LocalizeProvider Addressables 로드 실패!");
                IsInitialized = true;
                return;
            }
            
            _provider = handle.Result;
            await _provider.InitProvider();
            Debug.Log("[Localized] Initialized");
        }
    }
}