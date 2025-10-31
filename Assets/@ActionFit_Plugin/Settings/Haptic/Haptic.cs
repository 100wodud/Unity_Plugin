public static class Haptic
    {
        public static bool IsInitialized { get; private set; }

        private static bool _using;
        public static bool Using
        {
            get
            {
                if (!IsInitialized)
                {
                    _using = SettingData.Haptic;
                }

                return _using;
            }
            set
            {
                _using = value;
                SettingData.Haptic = value;
            }
        }

        public static void Initialize()
        {
#if UNITY_ANDROID && PROJECT_INSTALLED_HAPTIC || UNITY_IOS && PROJECT_INSTALLED_HAPTIC
            Vibration.Init();
            _using = SettingData.Haptic;
            IsInitialized = true;
#endif
        }

        public static void Weak()
        {
            if (!Using)
            {
                return;
            }
#if UNITY_ANDROID && PROJECT_INSTALLED_HAPTIC
            Vibration.VibrateAndroid(20);
#elif UNITY_IOS && PROJECT_INSTALLED_HAPTIC
            Vibration.VibrateIOS(ImpactFeedbackStyle.Light);
#endif
        }
        
        public static void Soft()
        {
            if (!Using)
            {
                return;
            }
            
#if UNITY_ANDROID && PROJECT_INSTALLED_HAPTIC
            Vibration.VibrateAndroid(30);
#elif UNITY_IOS && PROJECT_INSTALLED_HAPTIC
            Vibration.VibrateIOS(ImpactFeedbackStyle.Soft);
#endif
        }
        
        public static void Medium()
        {
            if (!Using)
            {
                return;
            }
            
#if UNITY_ANDROID && PROJECT_INSTALLED_HAPTIC
            Vibration.VibrateAndroid(60);
#elif UNITY_IOS && PROJECT_INSTALLED_HAPTIC
            Vibration.VibrateIOS(ImpactFeedbackStyle.Medium);
#endif
        }
        
        public static void Hard()
        {
            if (!Using)
            {
                return;
            }
            
#if UNITY_ANDROID && PROJECT_INSTALLED_HAPTIC
            Vibration.VibrateAndroid(100);
#elif UNITY_IOS && PROJECT_INSTALLED_HAPTIC
            Vibration.VibrateIOS(ImpactFeedbackStyle.Heavy);
#endif
        }
    }