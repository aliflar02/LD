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
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            UIManager.Instance.ShowDialoguePanel();
            UIManager.Instance.PlaySequence("SEQ_03_NEWSPAPER_READ");
        };
        UIManager.Instance.RegisterSequenceFinishedListener(sequenceId =>
        {
            if (sequenceId == "SEQ_03_NEWSPAPER_READ")
            {
                UIManager.Instance.GetItem(EItemType.NewsPaper);
                UIManager.Instance.HideFocusUI();
                UIManager.Instance.ShowItemPanel();
            }
        });
    }

}
