using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject lamp_hotspot;
    [SerializeField] private GameObject bed_hotspot;
    [SerializeField] private GameObject desk_hotspot;
    [SerializeField] private GameObject shelf_hotspot;

    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(lamp_hotspot).onClick = OnLampHotspotClicked;
        MUIEventListener.Get(bed_hotspot).onClick = OnBedHotspotClicked;
        MUIEventListener.Get(desk_hotspot).onClick = OnDeskHotspotClicked;
        MUIEventListener.Get(shelf_hotspot).onClick = OnShelfHotspotClicked;
    }


    private void OnLampHotspotClicked(GameObject go)
    {
        Debug.Log("点击了煤油灯热点，显示煤油灯界面");
        AudioManager.Instance.PlaySFX(ESFXType.Click);
        UIManager.Instance.ShowFocusUI(FocusPanelType.Lamp, lamp_hotspot.transform.position);
    }

    private void OnBedHotspotClicked(GameObject go)
    {
        Debug.Log("点击了床热点，显示床界面");
        if (GameManager.Instance.Model.GotItems.Contains(EItemType.NewsPaper))
        {
            Debug.Log("已经获得了报纸，不显示床界面");
            return;
        }
        AudioManager.Instance.PlaySFX(ESFXType.Click);
        UIManager.Instance.ShowFocusUI(FocusPanelType.Bed, bed_hotspot.transform.position);
    }

    private void OnDeskHotspotClicked(GameObject go)
    {
        Debug.Log("点击了书桌热点，显示书桌界面");
        AudioManager.Instance.PlaySFX(ESFXType.Click);
        UIManager.Instance.ShowFocusUI(FocusPanelType.Desk, desk_hotspot.transform.position);
    }

    private void OnShelfHotspotClicked(GameObject go)
    {
        Debug.Log("点击了书架热点，显示书架界面");
        AudioManager.Instance.PlaySFX(ESFXType.Click);
        UIManager.Instance.ShowFocusUI(FocusPanelType.Shelf, shelf_hotspot.transform.position);
    }


}
