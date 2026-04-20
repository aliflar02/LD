using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DeskUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject boxObject;
    [SerializeField] private GameObject notePanel;

    [SerializeField] private List<DeskNoteItemController> noteItemControllers;
    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(notePanel).onClick = _ =>
        {
            if (GameManager.Instance.Model.NoteItemDic.Count == 0)
            {
                Debug.Log("点击了笔记面板，但没有物品已使用");
                UIManager.Instance.HideItemPanel();
                UIManager.Instance.ShowDialoguePanel();
                UIManager.Instance.PlaySequence("SEQ_04_DESK_NOTEBOOK_HINT");
            }
        };
        UIManager.Instance.RegisterSequenceFinishedListener(sequenceId =>
        {
            if (sequenceId == "SEQ_04_DESK_NOTEBOOK_HINT")
            {
                UIManager.Instance.ShowItemPanel();
            }
        });

        MUIEventListener.Get(boxObject).onClick = _ =>
        {
            AudioManager.Instance.PlaySFX(ESFXType.Click);
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
            AudioManager.Instance.PlaySFX(ESFXType.Click);
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

        UIManager.Instance.RegisterSequenceFinishedListener(sequenceId =>
        {
            if (sequenceId == "SEQ_07_CLUE3_COMPLETE")
            {
                DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() =>
                {
                    //画面扭曲
                    UIManager.Instance.ShowFocusUI(FocusPanelType.Certificate, PopEffect: false);
                    UIManager.Instance.HidePersistentUI();
                });
            }
        });
    }
}
