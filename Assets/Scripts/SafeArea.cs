using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    RectTransform rt;
    Rect lastSafe;
    Vector2Int lastSize;

    void Awake() { rt = GetComponent<RectTransform>(); Apply(); }

    void Update()
    {
        if (Screen.safeArea != lastSafe || Screen.width != lastSize.x || Screen.height != lastSize.y)
            Apply();
    }

    void Apply()
    {
        Rect s = Screen.safeArea;
        lastSafe = s;
        lastSize = new Vector2Int(Screen.width, Screen.height);

        Vector2 min = s.position;
        Vector2 max = s.position + s.size;
        min.x /= Screen.width;  min.y /= Screen.height;
        max.x /= Screen.width;  max.y /= Screen.height;

        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}