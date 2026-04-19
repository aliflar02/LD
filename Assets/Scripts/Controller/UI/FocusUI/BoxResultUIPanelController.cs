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
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            UIManager.Instance.ShowDialoguePanel();
            UIManager.Instance.PlaySequence("SEQ_06_CERT_FOUND");
        };
        UIManager.Instance.RegisterSequenceFinishedListener(sequenceId =>
        {
            if (sequenceId == "SEQ_06_CERT_FOUND")
            {
                UIManager.Instance.HideFocusUI();
                UIManager.Instance.ShowItemPanel();
                UIManager.Instance.GetItem(EItemType.Photo);
            }
        });
    }

}
