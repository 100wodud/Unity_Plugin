using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ActionFit_Plugin.Localize
{
    [CreateAssetMenu(fileName = "LocalizeProvider", menuName = "Localize")]
    public class LocalizeProvider: ScriptableObject
    {
        private static Locale CurrentLanguage => LocalizationSettings.SelectedLocale;
        private const string TableRef = "LocalizeTable";
        private static int _currentLanguageIndex;
        public static int CurrentLangIndex => _currentLanguageIndex;
        public static event Action OnLanguageChanged;

        public async UniTask InitProvider()
        {
            int locale = PlayerPrefs.GetInt(TableRef, -1);
            if (locale != -1) _currentLanguageIndex = locale;
            else
            {
                _currentLanguageIndex = UnityEngine.Device.Application.systemLanguage switch
                {
                    SystemLanguage.English => 0,
                    SystemLanguage.Korean => 1,
                    SystemLanguage.Japanese => 2,
                    // SystemLanguage.ChineseSimplified => 3,
                    // SystemLanguage.ChineseTraditional => 4,
                    _ => 0
                };
            }
            await LocalizationSettings.InitializationOperation.Task;
            InitLocale();
        }

        private static void InitLocale()
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_currentLanguageIndex];
            PlayerPrefs.SetInt(TableRef, _currentLanguageIndex);
            PlayerPrefs.Save();
            OnLanguageChanged?.Invoke();
        }

        public static void ChangeLocale(int selectedLocale)
        {
            _currentLanguageIndex = selectedLocale;
            InitLocale();
        }

        public static string GetString(string key)
        {
            if(string.IsNullOrEmpty(key)) return string.Empty; 
            string s = LocalizationSettings.StringDatabase.GetLocalizedString(TableRef,key, CurrentLanguage);
            if (string.IsNullOrEmpty(s)) return $"Doesn't have Key : {key}";
            return s.Contains(@"\\") ? s.Replace(@"\\", "\n") : s;
        }

        public static string GetString(string key, int value) => string.Format(GetString(key),value);
        public static string GetString(string key, int value1, int value2) => string.Format(GetString(key),value1,value2);
    }
}