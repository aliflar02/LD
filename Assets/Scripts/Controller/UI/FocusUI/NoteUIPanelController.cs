using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject noteContentObject;

    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(noteContentObject).onClick = _ =>
        {
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            UIManager.Instance.ShowDialoguePanel();
            UIManager.Instance.PlaySequence("SEQ_05_SHELF_NOTE_FOUND");
        };

        UIManager.Instance.RegisterSequenceFinishedListener(sequenceId =>
        {
            if (sequenceId == "SEQ_05_SHELF_NOTE_FOUND")
            {
                UIManager.Instance.GetItem(EItemType.Note);
                UIManager.Instance.HideFocusUI();
                UIManager.Instance.ShowItemPanel();
            }
        });
    }

}
