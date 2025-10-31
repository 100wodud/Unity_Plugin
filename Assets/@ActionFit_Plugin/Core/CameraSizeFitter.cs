using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ActionFit_Plugin.Core
{
    public class CameraAspectFitter : MonoBehaviour
    {
        private void Awake()
        {
            Object.DontDestroyOnLoad(this);
        }
    }
}