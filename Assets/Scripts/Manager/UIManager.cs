using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

public interface IUIManager : ISingleton
{
    void ShowUI(string uiName);
    void HideUI(string uiName);
    void ShowFocusUI(FocusPanelType panelType);
    void HideFocusUI();//隐藏FocusUI使用这个，方便focusUI状态统一处理
    void GetItem(EItemType itemType);
    void UseItem(EItemType itemType);
    void ShowTips(string tips);
}

public class UIManager : MonoSingleton<UIManager>, IUIManager
{
    private Dictionary<string, GameObject> uiPanels = new();
    [SerializeField] private FocusUIPanelController focusUIPanelController;
    [SerializeField] private PersistentUIController persistentUIController;
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
    => focusUIPanelController.ShowPanel(panelType);
    public void HideFocusUI()
    {
        focusUIPanelController.gameObject.SetActive(false);
        GameManager.Instance.Model.CurrentFocusPanelType = FocusPanelType.None;
    }

    public void GetItem(EItemType itemType)
    {
        persistentUIController.GetItem(itemType);
        ShowTips("Got Item: " + itemType.ToString());
    }

    public void UseItem(EItemType itemType)
    {
        persistentUIController.UseItem(itemType);
        ShowTips("Used Item: " + itemType.ToString());
    }

    public void ShowTips(string tips)
    => persistentUIController.ShowTips(tips);

}
