using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject lampObject;
    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(lampObject).onClick = _ =>
        {
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            UIManager.Instance.HideFocusUI();
            UIManager.Instance.GetItem(EItemType.Lamp);
        };
    }
}
