using ActionFit_Plugin.Localize;
using ActionFit_Plugin.SDK;
using ActionFit_Plugin.Settings;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField] private Toggle vibeToggle;
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private TextMeshProUGUI textLevel;
    private void Start()
    {
        GameInit();
        
    }

    private async void GameInit()
    {
        vibeToggle.isOn = Setting.SetHaptic;
        bgmToggle.isOn = Setting.SetBGM;
        sfxToggle.isOn = Setting.SetSFX;
        textLevel.text = $"{PlayerData.ClearLevel} / {Setting.SetHaptic} / {Setting.SetBGM} / {SettingData.BGM}";
        vibeToggle.onValueChanged.AddListener((_)=>HapticOnOff());
        bgmToggle.onValueChanged.AddListener((_)=>BGMOnOff());
        sfxToggle.onValueChanged.AddListener((_)=>SFXOnOff());
        await UniTask.Delay(1000);
        SceneLoader.OnSceneLoadReady();
    }
    

    public void HapticOnOff()
    {
        Setting.SetHaptic = !Setting.SetHaptic;
        textLevel.text = $"{PlayerData.ClearLevel} / {Setting.SetHaptic} / {Setting.SetBGM} / {SettingData.BGM}";
    }
    
    public void BGMOnOff()
    {
        Setting.SetBGM = !Setting.SetBGM;
        textLevel.text = $"{PlayerData.ClearLevel} / {Setting.SetHaptic} / {Setting.SetBGM} / {SettingData.BGM}";
    }
    
    public void SFXOnOff()
    {
        Setting.SetSFX = !Setting.SetSFX;
        textLevel.text = $"{PlayerData.ClearLevel} / {Setting.SetHaptic} / {Setting.SetBGM} / {SettingData.BGM}";
    }

    public void HapticWeak()
    {
        Setting.HapticWeak();
    }
    public void HapticSoft()
    {
        Setting.HapticSoft();
    }
    public void HapticMedium()
    {
        Setting.HapticMedium();
    }
    public void HapticHard()
    {
        Setting.HapticHard();
    }

    public void PlayBGM()
    {
        Setting.PlayBGM(AudioLibraryMusic.BGM);
    }

    public void PlaySFX1()
    {
        Setting.PlaySFX(AudioLibrarySounds.Button);
    }
    public void PlaySFX2()
    {
        Setting.PlaySFX(AudioLibrarySounds.Drop);
    }
    public void PlaySFX3()
    {
        Setting.PlaySFX(AudioLibrarySounds.GameOver);
    }

    public void StopSound()
    {
        Setting.StopAllSound();
    }

    public void ChangeLangEn()
    {
        LocalizeProvider.ChangeLocale(0);
    }
    public void ChangeLangJp()
    {
        LocalizeProvider.ChangeLocale(1);
    }
    
    public void ChangeLangKr()
    {
        LocalizeProvider.ChangeLocale(2);
    }

    public void GotoLobbbbbbbbbbbbby()
    {
        SceneLoader.LoadSceneWithLoading(SceneName.LobbyScene);
    }
    
    
    public void GotoLobbbbbbbbbbbbbyNoLoading()
    {
        SceneLoader.LoadSceneWithoutLoading(SceneName.LobbyScene);
    }
#if ENABLE_APPLOVIN_SDK
    public void ShowInter()
    {
        SDKManager.ShowInterstitial(()=>{Debug.Log("응 전면광고");});
    }
    public void ShowReward()
    {
        SDKManager.ShowReward(()=>{Debug.Log("응 리워드광고");});
    }

    private bool _isBanner;
    public void ShowBanner()
    {
        if (_isBanner) SDKManager.HideBanner();
        else SDKManager.ShowBanner();
        _isBanner = !_isBanner;
    }

    public void ShowAppOpen()
    {
        SDKManager.ShowAppOpenAd();
    }
#endif
}
