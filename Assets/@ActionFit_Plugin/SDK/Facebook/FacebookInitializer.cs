#if false
using System;
using Cysharp.Threading.Tasks;
using Facebook.Unity;
using UnityEngine;

namespace ActionFit_Plugin.SDK.Facebook
{
    public static class FacebookInitializer
    {
        public static async UniTask Initialized()
        {
            if (!FB.IsInitialized)
            {
                FB.Init(FB.ActivateApp);
                bool success = await UniTask.WaitUntil(() => FB.IsInitialized).TimeoutWithoutException(TimeSpan.FromSeconds(3));
                if (!success) Debug.LogWarning("[FacebookInitializer] Failed Initialized");
                else Debug.Log("[FacebookInitializer] Initialized");
            }
            else FB.ActivateApp();
        }
    }
}
#endif
