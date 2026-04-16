using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 自定义事件触发器
/// </summary>
public class MUIEventListener : EventTrigger
{
    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onDrag;
    public VoidDelegate onDrop;
    public VoidDelegate onDeselect;
    public VoidDelegate onScroll;
    public VoidDelegate onMove;
    public VoidDelegate onInitializePotentialDrag;
    public VoidDelegate onBeginDrag;
    public VoidDelegate onEndDrag;
    public VoidDelegate onSubmit;
    public VoidDelegate onCancel;
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (onClick != null) onClick(gameObject);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (onDown != null) onDown(gameObject);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (onEnter != null) onEnter(gameObject);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (onExit != null) onExit(gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (onUp != null) onUp(gameObject);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (onSelect != null) onSelect(gameObject);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        base.OnUpdateSelected(eventData);
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (onDrag != null) onDrag(gameObject);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        if (onDrop != null) onDrop(gameObject);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (onDeselect != null) onDeselect(gameObject);
    }

    public override void OnScroll(PointerEventData eventData)
    {
        base.OnScroll(eventData);
        if (onScroll != null) onScroll(gameObject);
    }

    public override void OnMove(AxisEventData eventData)
    {
        base.OnMove(eventData);
        if (onMove != null) onMove(gameObject);
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
        if (onInitializePotentialDrag != null) onInitializePotentialDrag(gameObject);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (onBeginDrag != null) onBeginDrag(gameObject);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (onEndDrag != null) onEndDrag(gameObject);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        if (onSubmit != null) onSubmit(gameObject);
    }

    public override void OnCancel(BaseEventData eventData)
    {
        base.OnCancel(eventData);
        if (onCancel != null) onCancel(gameObject);
    }

    public static MUIEventListener Get(GameObject go)
    {
        MUIEventListener listener = go.GetComponent<MUIEventListener>();
        if (listener == null)
            listener = go.AddComponent<MUIEventListener>();
        return listener;
    }

}