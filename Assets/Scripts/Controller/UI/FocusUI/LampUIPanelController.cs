using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampUIPanelController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(gameObject).onClick = _ =>
        {
            UIManager.Instance.HideFocusUI();
            UIManager.Instance.GetItem(EItemType.Lamp);
        };
    }
}
