using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public static class MixerCheck
{
    const string Path = "Assets/Audio/MainMixer.mixer";

    [MenuItem("Tools/Audio/Check Mixer")]
    public static void Run()
    {
        AssetDatabase.ImportAsset(Path, ImportAssetOptions.ForceUpdate);
        var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(Path);
        if (mixer == null) { Debug.LogError("MixerCheck: mixer not found"); return; }

        foreach (var g in mixer.FindMatchingGroups("Master"))
            Debug.Log("MixerCheck group: " + g.name);
        foreach (string p in new[] { "MasterVol", "MusicVol", "SfxVol" })
            Debug.Log($"MixerCheck exposed {p}: {(mixer.GetFloat(p, out float v) ? v + " dB" : "MISSING")}");
        Debug.Log("MixerCheck snapshot Normal: " + (mixer.FindSnapshot("Normal") != null));
        Debug.Log("MixerCheck snapshot Paused: " + (mixer.FindSnapshot("Paused") != null));
        Debug.Log("MixerCheck update mode: " + mixer.updateMode);

        AssetDatabase.ForceReserializeAssets(new[] { Path });
        Debug.Log("MixerCheck: reserialized");
    }
}
