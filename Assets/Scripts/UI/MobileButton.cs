using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsHeld { get; private set; }

    bool pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsHeld = true;
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsHeld = false;
    }

    public bool ConsumePress()
    {
        if (!pressed) return false;
        pressed = false;
        return true;
    }

    void OnDisable()
    {
        IsHeld = false;
        pressed = false;
    }
}
