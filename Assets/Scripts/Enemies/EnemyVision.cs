using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] float viewDistance = 6f;
    [SerializeField] float viewAngle = 100f;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] Vector2 eyeOffset = new Vector2(0f, 0.2f);

    public Transform Player { get; private set; }
    public bool LastResult { get; private set; }

    int lastFacing = 1;

    void Awake()
    {
        PlayerHealth p = FindFirstObjectByType<PlayerHealth>();
        if (p != null) Player = p.transform;
    }

    public bool CanSee(int facing)
    {
        lastFacing = facing;
        LastResult = Check(facing);
        return LastResult;
    }

    bool Check(int facing)
    {
        if (Player == null) return false;

        Vector2 eye = (Vector2)transform.position + eyeOffset;
        Vector2 toPlayer = (Vector2)Player.position - eye;

        if (toPlayer.magnitude > viewDistance) return false;
        if (Vector2.Angle(new Vector2(facing, 0f), toPlayer) > viewAngle * 0.5f) return false;

        RaycastHit2D hit = Physics2D.Linecast(eye, Player.position, obstacleMask);
        Debug.DrawLine(eye, Player.position, hit.collider == null ? Color.yellow : Color.gray);
        return hit.collider == null;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 eye = transform.position + (Vector3)eyeOffset;
        Vector3 forward = new Vector3(lastFacing, 0f, 0f);
        Vector3 up = Quaternion.Euler(0, 0, viewAngle * 0.5f) * forward;
        Vector3 down = Quaternion.Euler(0, 0, -viewAngle * 0.5f) * forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(eye, eye + up * viewDistance);
        Gizmos.DrawLine(eye, eye + down * viewDistance);
        Gizmos.DrawWireSphere(eye, viewDistance);
    }
}
