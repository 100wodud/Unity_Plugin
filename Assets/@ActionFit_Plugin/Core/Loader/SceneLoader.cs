using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    GameScene,
    LobbyScene,
    LoadingScene,
}

public static class SceneLoader
{
    public static string TargetScene { get; private set; }
    private static Func<UniTask> OnLoadingSceneLoadBefore { get; set; }
    public static Action OnSceneLoadedComplete { get; private set; }
    private static UniTaskCompletionSource IsSceneReady { get; set; }
    public static void PrepareSceneInit() => IsSceneReady = new UniTaskCompletionSource();
    public static UniTask WaitUntilSceneReady() => IsSceneReady?.Task ?? UniTask.CompletedTask;
    
    // 활성화 된 Scene Initialize가 끝나고 실행시 Loading 내려감
    public static void OnSceneLoadReady() => IsSceneReady?.TrySetResult();

    public static async void LoadSceneWithLoading(SceneName targetScene, Func<UniTask> onLoadingLoadBefore = null, Action onCompleteAction = null)
    {
        TargetScene = targetScene.ToString();
        OnLoadingSceneLoadBefore = onLoadingLoadBefore;
        OnSceneLoadedComplete = onCompleteAction;
        if (OnLoadingSceneLoadBefore != null) await OnLoadingSceneLoadBefore.Invoke();
        SceneManager.LoadScene(SceneName.LoadingScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoadSceneWithoutLoading(SceneName targetScene, Action onCompleteAction = null)
    {
        OnSceneLoadedComplete = onCompleteAction;
        SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }
}
