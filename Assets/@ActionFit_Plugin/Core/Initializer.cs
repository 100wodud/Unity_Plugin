using System;
using ActionFit_Plugin.IAP;
using ActionFit_Plugin.Localize;
using ActionFit_Plugin.SDK;
using ActionFit_Plugin.SDK.Firebase;
using ActionFit_Plugin.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
            //ConfigureLogger();
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

#if ENABLE_APPLOVIN_SDK
            SDKManager.Initialized();
            await UniTask.WaitUntil(()=> SDKManager.IsInitialized);
#endif
            
#if ENABLE_FIREBASE_SDK
            await FirebaseInitializer.Initialized();
            await UniTask.WaitUntil(()=> FirebaseInitializer.IsInitialized);
#endif
            
#if ENABLE_IN_APP_PURCHASE
            await IAP.Initialized().Timeout(TimeSpan.FromSeconds(4));;
#endif
        }
        
        private void OnSceneLoadedComplete()
        {
            Debug.Log("GameScene 왔다능");
        }
    }
}
