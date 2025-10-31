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

            SceneLoader.PrepareSceneInit();
            var loadScene = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
            if (loadScene == null) return;
            loadScene.allowSceneActivation = false;
            await GameStartLoading();
            loadScene.allowSceneActivation = true;
            await UniTask.WaitUntil(() => loadScene.isDone);
            await SceneLoader.WaitUntilSceneReady();
            await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneLoader.OnSceneLoadedComplete?.Invoke();
        }


        #region Sample Method

        [SerializeField] private GameObject isAppOpen;
        [SerializeField] private GameObject isLoading;
    
        // 로딩 애니메이션이나 첫 로딩이 될수도 있을듯
        private async UniTask SceneChangeLoading()
        {
            isAppOpen.gameObject.SetActive(false);
            isLoading.gameObject.SetActive(true);
            await UniTask.Delay(1000);
        }
    
        private async UniTask GameStartLoading()
        {
            if (Initializer.Instance.AppFirstOpen)
            {
                await SceneChangeLoading();
                return;
            }
            Initializer.Instance.AppFirstOpen = true;
            isAppOpen.gameObject.SetActive(true);
            await UniTask.Delay(1000);
        }

        #endregion
    
    }
}
