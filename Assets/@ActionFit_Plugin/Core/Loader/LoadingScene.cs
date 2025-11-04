using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ActionFit_Plugin.Core.Loader
{
    public class LoadingScene : MonoBehaviour
    {
        private async void Awake()
        {
            string targetScene = SceneLoader.TargetScene;
            if (string.IsNullOrEmpty(targetScene)) return;
            
            if (!Initializer.Instance.AppFirstOpen) await GameStartLoading();
            
            SceneLoader.PrepareSceneInit();
            var loadScene = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
            if (loadScene == null) return;
            loadScene.allowSceneActivation = false;
            if (Initializer.Instance.AppFirstOpen) await SceneChangeLoading();
            loadScene.allowSceneActivation = true;
            await UniTask.WaitUntil(() => loadScene.isDone);
            await SceneLoader.WaitUntilSceneReady();
            await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneLoader.OnSceneLoadedComplete?.Invoke();
        }


        #region Sample Method

        [SerializeField] private GameObject isAppOpen;
        [SerializeField] private GameObject isLoading;
    
        // 씬 전환 애니메이션
        private async UniTask SceneChangeLoading()
        {
            isAppOpen.gameObject.SetActive(false);
            isLoading.gameObject.SetActive(true);
            await UniTask.Delay(1000);
        }
    
        // 첫 로딩
        private async UniTask GameStartLoading()
        {
            isAppOpen.gameObject.SetActive(false);
            isLoading.gameObject.SetActive(true);
            Initializer.Instance.AppFirstOpen = true;
            if (SceneLoader.OnGameStartLoading != null) await SceneLoader.OnGameStartLoading.Invoke();
            await UniTask.Delay(1000);
        }

        #endregion
    
    }
}
