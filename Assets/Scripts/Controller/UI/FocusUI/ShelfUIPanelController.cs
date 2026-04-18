using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShelfUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject noteObject;
    [SerializeField] private TMP_Text clueText;

    void Start()
    {
        if (GameManager.Instance.Model.GotItems.Contains(EItemType.Note))
        {
            //拿过笔记了后再次点开显示新线索
            noteObject.SetActive(false);
            clueText.gameObject.SetActive(true);
        }
        else
        {
            noteObject.SetActive(true);
            clueText.gameObject.SetActive(false);
        }

        MUIEventListener.Get(noteObject).onClick = _ =>
        {
            UIManager.Instance.HideFocusUI();
            UIManager.Instance.GetItem(EItemType.Note);
        };
    }
}
