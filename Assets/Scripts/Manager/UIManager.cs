using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

public interface IUIManager : ISingleton
{
    void ShowUI(string uiName);
    void HideUI(string uiName);
    void ShowFocusUI(FocusPanelType panelType);
}

public class UIManager : MonoSingleton<UIManager>, IUIManager
{
    private Dictionary<string, GameObject> uiPanels = new();
    [SerializeField] private FocusUIPanelController focusUIPanelController;
    public void OnSingletonInit()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            uiPanels[child.gameObject.name] = child.gameObject;
        }
        ShowUI("OverlayUI");
        ShowUI("MainUI");
        ShowUI("PersistentUI");

    }

    public void ShowUI(string uiName)
    {
        if (uiPanels.TryGetValue(uiName, out var panel))
        {
            panel.SetActive(true);
        }
        else
        {
            Debug.LogError($"UI Panel '{uiName}' not found!");
        }
    }

    public void HideUI(string uiName)
    {
        if (uiPanels.TryGetValue(uiName, out var panel))
        {
            panel.SetActive(false);
        }
        else
        {
            Debug.LogError($"UI Panel '{uiName}' not found!");
        }
    }

    public void ShowFocusUI(FocusPanelType panelType)
    {
        focusUIPanelController.ShowPanel(panelType);
    }
}
