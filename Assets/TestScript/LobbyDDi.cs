using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LobbyDDi : MonoBehaviour
{
    private async void Start()
    {
        await UniTask.Delay(1000);
        SceneLoader.OnSceneLoadReady();
    }

    public void GotoGammmmmmmmmmme()
    {
        SceneLoader.LoadSceneWithLoading(SceneName.GameScene);
    }
    
    public void GotoGammmmmmmmmmmeNoLoading()
    {
        SceneLoader.LoadSceneWithoutLoading(SceneName.GameScene);
    }
}
