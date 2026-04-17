using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public EItemType ItemType = EItemType.None;
    void Start()
    {
        MUIEventListener.Get(gameObject).onSelect = _ =>
        {
            GameManager.Instance.Model.CurrentItemType = ItemType;
            Debug.Log("select item: " + ItemType.ToString());
        };
        MUIEventListener.Get(gameObject).onDeselect = _ =>
        {
            if (GameManager.Instance.Model.CurrentFocusPanelType == FocusPanelType.Desk)
            {
                //如果当前聚焦界面是desk，不清空currentItemType，保持desk界面物品显示状态，直到切换到其他聚焦界面或者再次点击这个slot
                return;
            }
            if (GameManager.Instance.Model.CurrentItemType == ItemType)
            {
                Debug.Log("deselect item: " + ItemType.ToString());
                GameManager.Instance.Model.CurrentItemType = EItemType.None;

            }
        };
    }
    internal void Init(EItemType itemType)
    {
        this.ItemType = itemType;
        switch (itemType)
        {
            case EItemType.Lamp:
                text.text = "Lamp";
                break;
            case EItemType.NewsPaper:
                text.text = "Newspaper";
                break;
            case EItemType.Note:
                text.text = "Note";
                break;
            default:
                break;
        }
    }
}
