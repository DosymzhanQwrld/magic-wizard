#if UNITY_EDITOR && ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using NewTouchPhase = UnityEngine.InputSystem.TouchPhase;

public class RemoteTouchBridge : MonoBehaviour
{
    Touchscreen screen;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Create()
    {
        var go = new GameObject("RemoteTouchBridge");
        go.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(go);
        go.AddComponent<RemoteTouchBridge>();
    }

    void Update()
    {
        if (Input.touchCount == 0) return;
        if (screen == null) screen = InputSystem.AddDevice<Touchscreen>("RemoteTouch");

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            InputSystem.QueueStateEvent(screen, new TouchState
            {
                touchId = t.fingerId + 1,
                phase = Convert(t.phase),
                position = t.position,
                delta = t.deltaPosition,
                pressure = 1f
            });
        }
    }

    void OnDestroy()
    {
        if (screen != null) InputSystem.RemoveDevice(screen);
    }

    static NewTouchPhase Convert(UnityEngine.TouchPhase phase)
    {
        switch (phase)
        {
            case UnityEngine.TouchPhase.Began: return NewTouchPhase.Began;
            case UnityEngine.TouchPhase.Moved: return NewTouchPhase.Moved;
            case UnityEngine.TouchPhase.Stationary: return NewTouchPhase.Stationary;
            case UnityEngine.TouchPhase.Ended: return NewTouchPhase.Ended;
            default: return NewTouchPhase.Canceled;
        }
    }
}
#endif
