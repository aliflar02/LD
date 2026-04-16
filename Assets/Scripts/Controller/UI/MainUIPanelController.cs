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
    [SerializeField] private GameObject box_hotspot;

    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(lamp_hotspot).onClick += OnLampHotspotClicked;
        MUIEventListener.Get(bed_hotspot).onClick += OnBedHotspotClicked;
        MUIEventListener.Get(desk_hotspot).onClick += OnDeskHotspotClicked;
        MUIEventListener.Get(shelf_hotspot).onClick += OnShelfHotspotClicked;
        MUIEventListener.Get(box_hotspot).onClick += OnBoxHotspotClicked;
    }

    private void OnLampHotspotClicked(GameObject go)
    {
        Debug.Log("点击了煤油灯热点，显示煤油灯界面");
        UIManager.Instance.ShowFocusUI(FocusPanelType.Lamp);
    }

    private void OnBedHotspotClicked(GameObject go)
    {
        Debug.Log("点击了床热点，显示床界面");
        UIManager.Instance.ShowFocusUI(FocusPanelType.Bed);
    }

    private void OnDeskHotspotClicked(GameObject go)
    {
        Debug.Log("点击了书桌热点，显示书桌界面");
        UIManager.Instance.ShowFocusUI(FocusPanelType.Desk);
    }

    private void OnShelfHotspotClicked(GameObject go)
    {
        Debug.Log("点击了书架热点，显示书架界面");
        UIManager.Instance.ShowFocusUI(FocusPanelType.Shelf);
    }

    private void OnBoxHotspotClicked(GameObject go)
    {
        Debug.Log("点击了箱子热点，显示箱子界面");
        UIManager.Instance.ShowFocusUI(FocusPanelType.BoxResult);
    }

    void OnDestroy()
    {
        MUIEventListener.Get(lamp_hotspot).onClick -= OnLampHotspotClicked;
        MUIEventListener.Get(bed_hotspot).onClick -= OnBedHotspotClicked;
        MUIEventListener.Get(desk_hotspot).onClick -= OnDeskHotspotClicked;
        MUIEventListener.Get(shelf_hotspot).onClick -= OnShelfHotspotClicked;
        MUIEventListener.Get(box_hotspot).onClick -= OnBoxHotspotClicked;
    }
}
