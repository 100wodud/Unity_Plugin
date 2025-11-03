using System;
using UnityEngine;

namespace ActionFit_Plugin.SDK.Ads
{
    public class AdExtension 
    {
        public static void UpdateLastCooldownTime(string key)
        {
            double currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            PlayerPrefs.SetFloat(key, (float)currentTime);
            PlayerPrefs.Save();
        }

        public static bool InterCondition(string key, int cooldown)
        {
            double currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            double lastCooldownTime = PlayerPrefs.GetFloat(key, 0);
            return currentTime - lastCooldownTime >= cooldown;
        }
    }
}
