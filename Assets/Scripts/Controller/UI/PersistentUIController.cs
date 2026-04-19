using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EItemType
{
    None,
    Lamp,
    NewsPaper,
    Note,
    Photo,
}
public class PersistentUIController : MonoBehaviour
{
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private SlotController slotPrefab;

    [Obsolete]
    [SerializeField] private List<EItemType> gotItems = new();
    [SerializeField] private TMP_Text clickHintText;
    private float inventoryPanelShownX;
    private float inventoryPanelHiddenX;
    private bool isInventoryPanelVisible = true;

    void Awake()
    {
        slotPrefab.gameObject.SetActive(false);
        clickHintText.gameObject.SetActive(false);
    }

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        inventoryPanelShownX = inventoryPanel.anchoredPosition.x;
        inventoryPanelHiddenX = inventoryPanelShownX + inventoryPanel.rect.width;
        isInventoryPanelVisible = Mathf.Approximately(inventoryPanel.anchoredPosition.x, inventoryPanelShownX);
    }

    public void GetItem(EItemType itemType)
    {
        if (GameManager.Instance.Model.GotItems.Contains(itemType)) return;
        GameManager.Instance.Model.GotItems.Add(itemType);
        var slot = Instantiate(slotPrefab, inventoryPanel);
        slot.gameObject.SetActive(true);
        slot.Init(itemType);
    }

    public void UseItem(EItemType itemType)
    {
        if (itemType == EItemType.None) return;
        if (!GameManager.Instance.Model.GotItems.Contains(itemType)) return;
        //移除对应的slot
        foreach (Transform child in inventoryPanel)
        {
            var slot = child.GetComponent<SlotController>();
            if (slot != null && slot.ItemType == itemType)
            {
                var rt = child.GetComponent<RectTransform>();
                rt.DOScaleX(0f, 0.2f).From(1f).SetEase(Ease.InBack);
                rt.DOSizeDelta(new Vector2(rt.sizeDelta.x, 0f), 0.32f).SetEase(Ease.InBack).OnComplete(() => Destroy(child.gameObject));
                break;
            }
        }
    }

    public void ShowTips(string tips)
    {
        clickHintText.text = tips;
        clickHintText.gameObject.SetActive(true);
        DOTween.Sequence()
            .AppendInterval(1f)
            .AppendCallback(() => clickHintText.gameObject.SetActive(false));
    }

    public void HideItemPanel()
    {
        inventoryPanel.DOKill();
        inventoryPanel.DOAnchorPosX(inventoryPanelHiddenX, 0.5f).SetEase(Ease.OutCubic);
        isInventoryPanelVisible = false;
    }

    public void ShowItemPanel()
    {
        inventoryPanel.DOKill();
        inventoryPanel.DOAnchorPosX(inventoryPanelShownX, 0.5f).SetEase(Ease.OutBack);
        isInventoryPanelVisible = true;
    }

}
