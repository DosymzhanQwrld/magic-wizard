using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] RectTransform handle;
    [SerializeField] float range = 100f;
    [SerializeField] float deadZone = 0.15f;

    public Vector2 Value { get; private set; }

    RectTransform area;

    void Awake()
    {
        area = (RectTransform)transform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(area, eventData.position, eventData.pressEventCamera, out Vector2 local))
            return;

        Vector2 offset = Vector2.ClampMagnitude(local - area.rect.center, range);
        if (handle != null) handle.anchoredPosition = offset;

        Vector2 value = offset / range;
        Value = value.magnitude < deadZone ? Vector2.zero : value;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }

    void OnDisable()
    {
        Release();
    }

    void Release()
    {
        Value = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }
}
