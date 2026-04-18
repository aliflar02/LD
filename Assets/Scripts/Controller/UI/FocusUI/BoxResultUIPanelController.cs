using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxResultUIPanelController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(gameObject).onClick = _ =>
        {
            UIManager.Instance.GetItem(EItemType.Photo);
            UIManager.Instance.HideFocusUI();
        };
    }

}
