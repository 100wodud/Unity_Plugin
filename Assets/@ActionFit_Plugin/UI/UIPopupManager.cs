using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupManager : MonoBehaviour
{
    public static UIPopupManager Instance { get; private set; }
    public GameObject dim;
    [SerializeField] private List<BasePopup> popupList;
    [SerializeField] private GameObject dimUI;
    private Dictionary<string, BasePopup> _popupMap = new();
    public int order = 5;
    public Stack<BasePopup> popupStack = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        Initialized();
    }

    public void Initialized()
    {
        Init();
        RegisterPopups();
    }

    public void Init()
    {
        order = 5;
        dim.SetActive(false);
        DimUISwitch();
        popupStack.Clear();
    }

    private void RegisterPopups()
    {
        foreach (var popup in popupList)
        {
            if (popup != null && !_popupMap.ContainsKey(popup.name))
            {
                _popupMap.Add(popup.name, popup);
                popup.gameObject.SetActive(false);
            }
        }
    }

    public T GetPopup<T>(string name) where T : BasePopup
    {
        if (_popupMap.TryGetValue(name, out var popup))
        {
            return popup as T;
        }

        Debug.LogWarning($"[UIManager] {name} 팝업이 없습니다.");
        return null;
    }

    public void OpenPopup(string name, bool anime = true)
    {
        if (_popupMap.TryGetValue(name, out var popup))
        {
            popup.Open(anime);
        }
    }

    public void ClosePopup(string name)
    {
        if (_popupMap.TryGetValue(name, out var popup))
        {
            popup.Close();
        }
    }

    public void CloseAllPopups()
    {
        var temp = popupStack.ToArray();
        foreach (var popup in temp)
        {
            popup.gameObject.SetActive(false);
        }
        Init();
    }

    public void DimUISwitch(bool isTurnOn = false)
    {
        dimUI.SetActive(isTurnOn);
    }
}
