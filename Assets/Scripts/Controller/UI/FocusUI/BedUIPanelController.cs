using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject newspaperObject;
    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(newspaperObject).onClick = _ =>
        {
            UIManager.Instance.HideFocusUI();
            UIManager.Instance.GetItem(EItemType.NewsPaper);
        };
    }

}
