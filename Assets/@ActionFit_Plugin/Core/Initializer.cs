using ActionFit_Plugin.Localize;
using ActionFit_Plugin.SDK;
using ActionFit_Plugin.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ActionFit_Plugin.Core
{
    public class Initializer : MonoBehaviour
    {
        public static Initializer Instance { get; private set; }
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
            Initialized();
        }

        private void Initialized()
        {
            SceneLoader.LoadSceneWithLoading(SceneName.GameScene,null, OnLoadingLoadBefore, OnSceneLoadedComplete);
        }

        #endregion

        private async UniTask OnLoadingLoadBefore()
        {
            PlayerData.Init();
            await UniTask.WaitUntil(()=> PlayerData.IsInitialized);
            
            Setting.Initialized();
            await UniTask.WaitUntil(()=> Setting.IsInitialized);
            
            LocalizeInitializer.Initialized();
            await UniTask.WaitUntil(()=> LocalizeInitializer.IsInitialized);

#if ENABLE_APPLOVIN_SDK
            SDKManager.Initialized();
            await UniTask.WaitUntil(()=> SDKManager.IsInitialized);
#endif
        }
        
        private void OnSceneLoadedComplete()
        {
            Debug.Log("GameScene 왔다능");
        }
    }
}
