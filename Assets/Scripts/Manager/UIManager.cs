using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;
using UnityEngine.Events;

public interface IUIManager : ISingleton
{
    void ShowUI(string uiName);
    void HideUI(string uiName);
    void ShowFocusUI(FocusPanelType panelType, Vector3 startPos = default(Vector3));
    void HideFocusUI();//隐藏FocusUI使用这个，方便focusUI状态统一处理
    void HidePersistentUI();
    void ShowItemPanel();
    void HideItemPanel();
    void GetItem(EItemType itemType);
    void UseItem(EItemType itemType);
    void ShowTips(string tips);
    void ShowDialoguePanel();
    void PlaySequence(string sequenceId);
    void PlaySequence(DialogueSequence sequence);
    void RegisterSequenceFinishedListener(UnityEngine.Events.UnityAction<string> listener);
}

public class UIManager : MonoSingleton<UIManager>, IUIManager
{
    private Dictionary<string, GameObject> uiPanels = new();
    [SerializeField] private FocusUIPanelController focusUIPanelController;
    [SerializeField] private PersistentUIController persistentUIController;
    [SerializeField] private DialogueController dialogueController;
    public void OnSingletonInit()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            uiPanels[child.gameObject.name] = child.gameObject;
        }
        ShowUI("StartUI");
        dialogueController.sequenceFinished.AddListener(sequenceId =>
        {
            dialogueController.gameObject.SetActive(false);
        });

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

    public void ShowFocusUI(FocusPanelType panelType, Vector3 startPos = default(Vector3))
    {
        focusUIPanelController.ShowPanel(panelType, startPos);
        if (panelType != FocusPanelType.Desk)
        {
            HideItemPanel();
        }
    }
    public void HideFocusUI()
    {
        ShowItemPanel();
        focusUIPanelController.gameObject.SetActive(false);
        GameManager.Instance.Model.CurrentFocusPanelType = FocusPanelType.None;
    }

    public void HidePersistentUI()
    => persistentUIController.gameObject.SetActive(false);

    public void GetItem(EItemType itemType)
    {
        persistentUIController.GetItem(itemType);
    }

    public void UseItem(EItemType itemType)
    {
        persistentUIController.UseItem(itemType);
    }

    public void ShowTips(string tips)
        => persistentUIController.ShowTips(tips);

    public void ShowItemPanel()
        => persistentUIController.ShowItemPanel();

    public void HideItemPanel()
        => persistentUIController.HideItemPanel();

    public void PlaySequence(string sequenceId)
        => dialogueController.PlaySequence(sequenceId);

    public void PlaySequence(DialogueSequence sequence)
        => dialogueController.PlaySequence(sequence);

    public void RegisterSequenceFinishedListener(UnityAction<string> listener)
    {
        dialogueController.sequenceFinished.AddListener(listener);
    }

    public void ShowDialoguePanel()
    {
        UIManager.Instance.HideItemPanel();
        dialogueController.gameObject.SetActive(true);
    }

}
