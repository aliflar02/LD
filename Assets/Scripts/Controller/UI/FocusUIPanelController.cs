using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum FocusPanelType
{
    None,
    Lamp,
    Bed,
    Desk,
    Shelf,
    Password,
    BoxResult,
    Note,
    Certificate,
}

public class FocusUIPanelController : MonoBehaviour
{
    [SerializeField] private Image BgImage;
    [SerializeField] private GameObject lampPanel;
    [SerializeField] private GameObject bedPanel;
    [SerializeField] private GameObject deskPanel;
    [SerializeField] private GameObject shelfPanel;
    [SerializeField] private GameObject passwordPanel;
    [SerializeField] private GameObject boxResultPanel;
    [SerializeField] private GameObject notePanel;
    [SerializeField] private GameObject certificatePanel;
    // Start is called before the first frame update
    void Awake()
    {
        HideAllPanels();
    }
    void Start()
    {
        MUIEventListener.Get(BgImage.gameObject).onClick = _ =>
        {
            UIManager.Instance.HideFocusUI();
            AudioManager.Instance.PlaySFX(ESFXType.Click);
        };
    }
    private void HideAllPanels()
    {
        lampPanel.SetActive(false);
        bedPanel.SetActive(false);
        deskPanel.SetActive(false);
        shelfPanel.SetActive(false);
        passwordPanel.SetActive(false);
        boxResultPanel.SetActive(false);
        notePanel.SetActive(false);
        certificatePanel.SetActive(false);
    }

    public void ShowPanel(FocusPanelType panelType, Vector3 startPos = default(Vector3), bool PopEffect = true)
    {
        gameObject.SetActive(true);
        HideAllPanels();
        GameManager.Instance.Model.CurrentFocusPanelType = panelType;
        Debug.Log($"显示聚焦界面: {panelType}");
        var targetPanel = panelType switch
        {
            FocusPanelType.Lamp => lampPanel,
            FocusPanelType.Bed => bedPanel,
            FocusPanelType.Desk => deskPanel,
            FocusPanelType.Shelf => shelfPanel,
            FocusPanelType.Password => passwordPanel,
            FocusPanelType.BoxResult => boxResultPanel,
            FocusPanelType.Note => notePanel,
            FocusPanelType.Certificate => certificatePanel,
            _ => null
        };
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            if (startPos != default(Vector3))
            {
                targetPanel.transform.position = startPos;
                targetPanel.transform.DOLocalMove(Vector3.zero, 0.3f);
            }
            else
            {
                targetPanel.transform.localPosition = Vector3.zero;
            }
            if (PopEffect)
            {
                targetPanel.transform.DOScale(1f, 0.3f).From(0f).SetEase(Ease.OutBack);
            }
            else
            {
                targetPanel.transform.localScale = Vector3.one;
            }
        }
        BgImage.DOFade(1f, 0.3f).From(0f);
    }
}
