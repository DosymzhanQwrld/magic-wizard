using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothTime = 0.15f;
    [SerializeField] float minX = 4f;
    [SerializeField] float maxX = 56f;
    [SerializeField] float baseY = 3.5f;
    [SerializeField] float lookUpMargin = 1f;

    float velocityX;
    float velocityY;

    void LateUpdate()
    {
        if (target == null) return;
        float goalX = Mathf.Clamp(target.position.x, minX, maxX);
        float goalY = Mathf.Max(baseY, target.position.y + lookUpMargin);
        Vector3 p = transform.position;
        p.x = Mathf.SmoothDamp(p.x, goalX, ref velocityX, smoothTime);
        p.y = Mathf.SmoothDamp(p.y, goalY, ref velocityY, smoothTime);
        transform.position = p;
    }
}
