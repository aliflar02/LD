using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject sceneRoot;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private float sceneMoveSpeed = 800f;
    [SerializeField] private GameObject lamp_hotspot;
    [SerializeField] private GameObject bed_hotspot;
    [SerializeField] private GameObject desk_hotspot;
    [SerializeField] private GameObject shelf_hotspot;

    private RectTransform _sceneRootRect;
    private Canvas _rootCanvas;
    private Camera _uiCamera;
    private bool _isHoldingLeft;
    private bool _isHoldingRight;
    private readonly Vector3[] _sceneCorners = new Vector3[4];

    // Start is called before the first frame update
    void Start()
    {
        _sceneRootRect = sceneRoot != null ? sceneRoot.GetComponent<RectTransform>() : null;
        if (_sceneRootRect != null)
        {
            _rootCanvas = _sceneRootRect.GetComponentInParent<Canvas>()?.rootCanvas;
            _uiCamera = _rootCanvas != null && _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _rootCanvas?.worldCamera;
        }

        MUIEventListener.Get(lamp_hotspot).onClick = OnLampHotspotClicked;
        MUIEventListener.Get(bed_hotspot).onClick = OnBedHotspotClicked;
        MUIEventListener.Get(desk_hotspot).onClick = OnDeskHotspotClicked;
        MUIEventListener.Get(shelf_hotspot).onClick = OnShelfHotspotClicked;

        var leftArrowListener = MUIEventListener.Get(leftArrow);
        leftArrowListener.onDown = OnLeftArrowDown;
        leftArrowListener.onUp = OnLeftArrowUp;
        leftArrowListener.onExit = OnLeftArrowUp;

        var rightArrowListener = MUIEventListener.Get(rightArrow);
        rightArrowListener.onDown = OnRightArrowDown;
        rightArrowListener.onUp = OnRightArrowUp;
        rightArrowListener.onExit = OnRightArrowUp;
    }

    private void Update()
    {
        float direction = 0f;
        if (_isHoldingLeft) direction += 1f;
        if (_isHoldingRight) direction -= 1f;

        if (Mathf.Approximately(direction, 0f))
        {
            return;
        }

        MoveScene(direction);
    }

    private void OnDisable()
    {
        _isHoldingLeft = false;
        _isHoldingRight = false;
    }

    private void OnLeftArrowDown(GameObject go)
    {
        Debug.Log("按下左箭头，场景持续左移");
        _isHoldingLeft = true;
        _isHoldingRight = false;
    }

    private void OnRightArrowDown(GameObject go)
    {
        Debug.Log("按下右箭头，场景持续右移");
        _isHoldingRight = true;
        _isHoldingLeft = false;
    }

    private void OnLeftArrowUp(GameObject go)
    {
        _isHoldingLeft = false;
    }

    private void OnRightArrowUp(GameObject go)
    {
        _isHoldingRight = false;
    }

    private void MoveScene(float direction)
    {
        if (_sceneRootRect == null)
        {
            return;
        }

        float desiredDeltaCanvas = direction * sceneMoveSpeed * Time.deltaTime;
        if (Mathf.Approximately(desiredDeltaCanvas, 0f))
        {
            return;
        }

        float allowedDeltaCanvas = ClampDeltaToScreenBounds(desiredDeltaCanvas, direction);
        if (Mathf.Approximately(allowedDeltaCanvas, 0f))
        {
            return;
        }

        Vector2 anchoredPos = _sceneRootRect.anchoredPosition;
        anchoredPos.x += allowedDeltaCanvas;
        _sceneRootRect.anchoredPosition = anchoredPos;
    }

    private float ClampDeltaToScreenBounds(float desiredDeltaCanvas, float direction)
    {
        if (_rootCanvas == null)
        {
            return desiredDeltaCanvas;
        }

        float scaleFactor = Mathf.Max(0.0001f, _rootCanvas.scaleFactor);
        float desiredDeltaScreen = desiredDeltaCanvas * scaleFactor;

        GetSceneScreenEdges(out float leftEdge, out float rightEdge);

        if (direction < 0f)
        {
            // 向左移动时，限制 sceneRoot 的右边缘不小于屏幕右边缘
            float maxLeftMoveScreen = Mathf.Max(0f, rightEdge - Screen.width);
            float clampedDeltaScreen = Mathf.Max(-maxLeftMoveScreen, desiredDeltaScreen);
            return clampedDeltaScreen / scaleFactor;
        }

        // 向右移动时，限制 sceneRoot 的左边缘不大于屏幕左边缘
        float maxRightMoveScreen = Mathf.Max(0f, -leftEdge);
        float clampedRightDeltaScreen = Mathf.Min(maxRightMoveScreen, desiredDeltaScreen);
        return clampedRightDeltaScreen / scaleFactor;
    }

    private void GetSceneScreenEdges(out float leftEdge, out float rightEdge)
    {
        _sceneRootRect.GetWorldCorners(_sceneCorners);

        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(_uiCamera, _sceneCorners[0]);
        Vector2 topLeft = RectTransformUtility.WorldToScreenPoint(_uiCamera, _sceneCorners[1]);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(_uiCamera, _sceneCorners[2]);
        Vector2 bottomRight = RectTransformUtility.WorldToScreenPoint(_uiCamera, _sceneCorners[3]);

        leftEdge = Mathf.Min(bottomLeft.x, topLeft.x);
        rightEdge = Mathf.Max(topRight.x, bottomRight.x);
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
        UIManager.Instance.ShowFocusUI(FocusPanelType.Bed, PopEffect: false);
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
        UIManager.Instance.ShowFocusUI(FocusPanelType.Shelf, PopEffect: false);
    }


}
