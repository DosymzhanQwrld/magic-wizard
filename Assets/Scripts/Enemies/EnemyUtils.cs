using UnityEngine;

public static class EnemyUtils
{
    public static bool IsOnScreen(Vector3 worldPosition, float margin = 0.05f)
    {
        Camera cam = Camera.main;
        if (cam == null) return true;
        Vector3 v = cam.WorldToViewportPoint(worldPosition);
        return v.x > margin && v.x < 1f - margin && v.y > margin && v.y < 1f - margin;
    }
}
