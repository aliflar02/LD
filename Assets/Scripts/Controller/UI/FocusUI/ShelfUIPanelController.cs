using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject noteObject;

    void Start()
    {
        MUIEventListener.Get(noteObject).onClick = _ =>
        {
            UIManager.Instance.HideFocusUI();
            UIManager.Instance.GetItem(EItemType.Note);
        };
    }
}
