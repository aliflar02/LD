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
    Note
}
public class PersistentUIController : MonoBehaviour
{
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private SlotController slotPrefab;

    [Obsolete]
    [SerializeField] private List<EItemType> gotItems = new();
    [SerializeField] private TMP_Text clickHintText;
    [SerializeField] private Button btnExit;
    void Awake()
    {
        slotPrefab.gameObject.SetActive(false);
        clickHintText.gameObject.SetActive(false);
    }
    void Start()
    {
        btnExit.onClick.AddListener(() =>
        {
            if (Application.isEditor)
                UnityEditor.EditorApplication.isPlaying = false;
            else
                Application.Quit();
        });
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
                Destroy(child.gameObject);
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
}
