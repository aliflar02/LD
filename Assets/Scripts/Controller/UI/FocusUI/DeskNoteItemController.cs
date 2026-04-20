using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeskNoteItemController : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;
    [SerializeField] private List<Sprite> itemSprites;

    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(gameObject).onClick = _ =>
        {
            var model = GameManager.Instance.Model;
            var currentItemType = model.CurrentItemType;
            print("click note item, currentItemType: " + currentItemType);
            if (currentItemType == EItemType.None)
            {
                Debug.Log("当前没有选中的物品，无法使用");
                return;
            }
            if (!model.NoteItemDic.ContainsKey(_.name))
            {
                Debug.Log("不包含这个gameobject，可以使用物品");
                model.NoteItemDic[_.name] = currentItemType;
                Init(currentItemType);
                UIManager.Instance.UseItem(currentItemType);
                if (model.NoteItemDic.Count == 1) //第一次使用物品，打开对话
                {
                    UIManager.Instance.HideItemPanel();
                    UIManager.Instance.ShowDialoguePanel();
                    UIManager.Instance.PlaySequence("SEQ_04_DESK_NOTEBOOK_HINT");
                }
                else if (model.NoteItemDic.Count == 4) //已获得所有物品
                {
                    UIManager.Instance.HideItemPanel();
                    UIManager.Instance.ShowDialoguePanel();
                    UIManager.Instance.PlaySequence("SEQ_07_CLUE3_COMPLETE");
                }
            }
            else
            {
                string usedItemType = "";
                foreach (var item in model.NoteItemDic)
                {
                    usedItemType += item.Value.ToString() + " ";
                }
                Debug.Log("已经包含这个gameobject了，切换物品");
                Debug.Log(_.name);
                Debug.Log("已使用的物品类型: " + usedItemType);
            }
            model.CurrentItemType = EItemType.None;

            UIManager.Instance.RegisterSequenceFinishedListener(sequenceId =>
            {
                if (sequenceId == "SEQ_04_DESK_NOTEBOOK_HINT")
                {
                    UIManager.Instance.ShowItemPanel();
                }
            });
        };
    }

    public void Init(EItemType itemType)
    {
        Debug.Log("Todo: 加动效");
        switch (itemType)
        {
            case EItemType.Lamp:
                image.sprite = itemSprites[0];
                break;
            case EItemType.NewsPaper:
                image.sprite = itemSprites[1];
                break;
            case EItemType.Note:
                image.sprite = itemSprites[2];
                break;
            case EItemType.Photo:
                image.sprite = itemSprites[3];
                break;
            default:
                break;
        }
        image.DOFade(1f, 0.5f).From(0f).SetEase(Ease.OutCubic);
    }
}
