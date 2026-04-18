using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeskNoteItemController : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

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
                if (model.NoteItemDic.Count == 4) //已获得所有物品
                {
                    //打开对话：这里好黑啊……
                    //对话结束后画面扭曲，然后黑屏1s，点击后打开证明书
                    UIManager.Instance.ShowFocusUI(FocusPanelType.Certificate);
                    UIManager.Instance.HidePersistentUI();
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
        };
    }

    public void Init(EItemType itemType)
    {
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
            case EItemType.Photo:
                text.text = "Photo";
                break;
            default:
                break;
        }
    }
}
