using System;
using ActionFit_Plugin.Data.Scripts;
using ActionFit_Plugin.Localize;
using ActionFit_Plugin.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if ENABLE_APPLOVIN_SDK
using ActionFit_Plugin.SDK;
#endif
#if ENABLE_FIREBASE_SDK
using ActionFit_Plugin.SDK.Firebase;
#endif
#if ENABLE_GAN_SDK
using GameAnalyticsSDK;
#endif
#if ENABLE_SINGULAR_SDK
using Singular;
#endif
#if ENABLE_IN_APP_PURCHASE
using ActionFit_Plugin.IAP;
#endif

namespace ActionFit_Plugin.Core
{
    public class Initializer : MonoBehaviour
    {
        public static Initializer Instance { get; private set; }
#if ENABLE_IN_APP_PURCHASE
        public IAPManager IAP;
#endif
        public bool AppFirstOpen  { get; set; } = false;

        #region Initialize

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            ConfigureLogger();
            Initialized();
        }

        private void Initialized()
        {
            SceneLoader.LoadSceneWithLoading(SceneName.GameScene,null, OnLoadingLoadBefore, OnSceneLoadedComplete);
        }
        
        private void ConfigureLogger()
        {
#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = false;
#else
            Debug.unityLogger.logEnabled = true;
#endif
        }

        #endregion

        private async UniTask OnLoadingLoadBefore()
        {
            PlayerData.Initialized();
            await UniTask.WaitUntil(()=> PlayerData.IsInitialized);
            Setting.Initialized();
            await UniTask.WaitUntil(()=> Setting.IsInitialized);
            LocalizeInitializer.Initialized();
            await UniTask.WaitUntil(()=> LocalizeInitializer.IsInitialized);
            
            #region SDK & IAP

#if ENABLE_APPLOVIN_SDK
            SDKManager.Initialized();
            await UniTask.WaitUntil(()=> SDKManager.IsInitialized);
#endif
            
#if ENABLE_SINGULAR_SDK
            try
            {
                SingularSDK.InitializeSingularSDK();
                await UniTask.WaitUntil(()=> SingularSDK.Initialized).Timeout(TimeSpan.FromSeconds(2));
            }
            catch (Exception e)
            {
                Debug.LogError("[Singular] Fail Initialized: " + e.Message);
            }
#endif

#if ENABLE_GAN_SDK
            try
            {
                GameAnalytics.StartSession();
                await UniTask.WaitUntil(()=> GameAnalytics.Initialized).Timeout(TimeSpan.FromSeconds(2));
            }
            catch (Exception e)
            {
                Debug.LogError("[GAN] Fail Initialized: " + e.Message);
            }
#endif
            
#if ENABLE_FIREBASE_SDK
            await FirebaseInitializer.Initialized();
            await UniTask.WaitUntil(() => FirebaseInitializer.IsInitialized);
#endif
            
#if ENABLE_IN_APP_PURCHASE
            await IAP.Initialized();
#endif

            #endregion
        }
        
        private void OnSceneLoadedComplete()
        {
            Debug.Log("GameScene 왔다능");
        }
    }
}
