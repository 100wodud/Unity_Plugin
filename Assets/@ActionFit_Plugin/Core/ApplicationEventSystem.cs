using System;
using UnityEngine;

namespace ActionFit_Plugin.Core
{
    public class ApplicationEventSystem : MonoBehaviour
    {
        public static event Action OnAppStateForeground;
        public static event Action OnAppStateBackground;

        private static bool _isForeground = true;
    
        public static bool IsWatchingAd = false;

        private void OnEnable()
        {
            OnAppStateForeground += OnAppResume;
            OnAppStateBackground += OnAppPause;
        }

        private void OnDisable()
        {
            OnAppStateForeground -= OnAppResume;
            OnAppStateBackground -= OnAppPause;
        }


        #region Event Methods

        private void OnApplicationFocus(bool focus)
        {
            HandleAppStateChange(focus);
        }

        private void OnApplicationPause(bool pause)
        {
            HandleAppStateChange(!pause);
        }

        private void HandleAppStateChange(bool isForeground)
        {
            if (IsWatchingAd)
            {
                Debug.Log("광고 봤어");
                return;
            }
        
            if (_isForeground == isForeground) return;
            _isForeground = isForeground;
    
            if (_isForeground)
            {
                OnAppStateForeground?.Invoke();
            }
            else
            {
                OnAppStateBackground?.Invoke();
            }
        }

        private void OnAppResume()
        {
            Debug.Log("앱이 포그라운드로 돌아왔다오!");
        }

        private void OnAppPause()
        {
            Debug.Log("앱이 백그라운드로 이동했다오!");
        }

        #endregion
    }
}
