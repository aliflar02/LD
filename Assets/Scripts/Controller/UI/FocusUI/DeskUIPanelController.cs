using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject boxObject;

    [SerializeField] private List<DeskNoteItemController> noteItemControllers;
    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(boxObject).onClick = _ =>
        {
            if (GameManager.Instance.Model.GotItems.Contains(EItemType.Photo))
            {
                Debug.Log("已经获得了照片，显示箱子结果界面");
                UIManager.Instance.ShowFocusUI(FocusPanelType.BoxResult);
            }
            else
            {
                Debug.Log("没有获得照片，显示密码界面");
                UIManager.Instance.ShowFocusUI(FocusPanelType.Password);
            }
        };
        MUIEventListener.Get(gameObject).onClick = _ =>
        {
            UIManager.Instance.HideFocusUI();
        };

        foreach (var noteItemController in GameManager.Instance.Model.NoteItemDic)
        {

            if (noteItemControllers.Exists(controller => controller.gameObject.name == noteItemController.Key))
            {
                var controller = noteItemControllers.Find(c => c.gameObject.name == noteItemController.Key);
                controller.Init(noteItemController.Value);
            }
        }
    }
}
