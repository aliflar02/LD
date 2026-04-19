using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShelfUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject noteObject;
    [SerializeField] private TMP_Text clueText;

    void Awake()
    {
        noteObject.SetActive(true);
        clueText.gameObject.SetActive(false);
    }
    void Start()
    {
        MUIEventListener.Get(gameObject).onClick = _ =>
        {
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            UIManager.Instance.HideFocusUI();
        };

        MUIEventListener.Get(noteObject).onClick = _ =>
        {
            UIManager.Instance.ShowFocusUI(FocusPanelType.Note);
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            noteObject.SetActive(false);
            clueText.gameObject.SetActive(true);
        };
    }
}
