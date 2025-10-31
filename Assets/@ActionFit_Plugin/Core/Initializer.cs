using ActionFit_Plugin.Localize;
using ActionFit_Plugin.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ActionFit_Plugin.Core
{
    public class Initializer : MonoBehaviour
    {
        public static Initializer Instance { get; private set; }
        public bool AppFirstOpen  { get; set; } = false;
        private void Awake()
        {
            Initialized();
        }

        private void Initialized()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
            SceneLoader.LoadSceneWithLoading(SceneName.GameScene, OnLoadingLoadBefore, OnSceneLoadedComplete);
        }

        private async UniTask OnLoadingLoadBefore()
        {
            PlayerData.Init();
            await UniTask.WaitUntil(()=> PlayerData.IsInitialized);
            
            Setting.Initialized();
            await UniTask.WaitUntil(()=> Setting.IsInitialized);
            
            LocalizeInitializer.Initialized();
            await UniTask.WaitUntil(()=> LocalizeInitializer.IsInitialized);

#if ACTIONFIT_SDK_INSTALL
            await SDK.Initialized();
#endif
            
#if ACTIONFIT_IAP_INSTALL
            await IAP.Initialized();
#endif
        }
        
        private void OnSceneLoadedComplete()
        {
            Debug.Log("GameScene 왔다능");
        }
    }
}
