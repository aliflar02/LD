using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FocusPanelType
{
    None,
    Lamp,
    Bed,
    Desk,
    Shelf,
    Password,
    BoxResult,
}

public class FocusUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject lampPanel;
    [SerializeField] private GameObject bedPanel;
    [SerializeField] private GameObject deskPanel;
    [SerializeField] private GameObject shelfPanel;
    [SerializeField] private GameObject passwordPanel;
    [SerializeField] private GameObject boxResultPanel;
    // Start is called before the first frame update
    void Awake()
    {
        HideAllPanels();
    }
    private void HideAllPanels()
    {
        lampPanel.SetActive(false);
        bedPanel.SetActive(false);
        deskPanel.SetActive(false);
        shelfPanel.SetActive(false);
        passwordPanel.SetActive(false);
        boxResultPanel.SetActive(false);
    }

    public void ShowPanel(FocusPanelType panelType)
    {
        gameObject.SetActive(true);
        HideAllPanels();
        Debug.Log($"显示聚焦界面: {panelType}");
        switch (panelType)
        {
            case FocusPanelType.Lamp:
                lampPanel.SetActive(true);
                print("显示煤油灯界面");
                break;
            case FocusPanelType.Bed:
                bedPanel.SetActive(true);
                break;
            case FocusPanelType.Desk:
                deskPanel.SetActive(true);
                break;
            case FocusPanelType.Shelf:
                shelfPanel.SetActive(true);
                break;
            case FocusPanelType.Password:
                passwordPanel.SetActive(true);
                break;
            case FocusPanelType.BoxResult:
                boxResultPanel.SetActive(true);
                break;
        }
    }
}
