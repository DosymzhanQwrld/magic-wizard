using UnityEngine;

public class SaveAutosave : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        SaveSystem.Load();
        var go = new GameObject("SaveAutosave");
        DontDestroyOnLoad(go);
        go.AddComponent<SaveAutosave>();
        Debug.Log("Save file: " + SaveSystem.FilePath);
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) SaveSystem.Save();
    }

    void OnApplicationQuit()
    {
        SaveSystem.Save();
    }
}
