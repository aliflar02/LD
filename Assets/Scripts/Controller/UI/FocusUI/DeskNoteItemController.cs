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
            if (!model.NoteItemDic.ContainsKey(_.name))
            {
                Debug.Log("不包含这个gameobject，可以使用物品");
                model.NoteItemDic[_.name] = currentItemType;
                Init(currentItemType);
                UIManager.Instance.UseItem(currentItemType);
            }
            else
            {
                Debug.Log("已经包含这个gameobject了，切换物品");
                Debug.Log(_.name);
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
            default:
                break;
        }
    }
}
