using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] float factor = 0.2f;
    [SerializeField] float tileWidth = 24f;

    Transform cam;
    float baseY;

    void Start()
    {
        cam = transform.parent;
        baseY = transform.localPosition.y;
    }

    void LateUpdate()
    {
        if (cam == null) return;
        float offset = Mathf.Repeat(cam.position.x * factor, tileWidth);
        transform.localPosition = new Vector3(-offset, baseY, transform.localPosition.z);
    }
}
