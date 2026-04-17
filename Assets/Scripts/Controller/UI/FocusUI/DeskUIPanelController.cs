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
            UIManager.Instance.ShowFocusUI(FocusPanelType.BoxResult);
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
