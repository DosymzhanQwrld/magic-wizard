using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

// Tools > Audio > Setup Audio Library
// Sets import settings for every clip in Assets/Audio and fills Assets/Resources/AudioLibrary.asset by file name:
// "hit_enemy_1" and "hit_enemy_2" both go into hitEnemy (random variation). Run it again after adding clips.
public static class AudioSetup
{
    const string AudioFolder = "Assets/Audio";
    const string LibraryPath = "Assets/Resources/AudioLibrary.asset";
    const string HeartSource = "Assets/Art/Generated/Heart.png";
    const string HeartTarget = "Assets/Resources/Heart.png";

    [MenuItem("Tools/Audio/Setup Audio Library")]
    public static void Run()
    {
        var clips = new Dictionary<string, AudioClip>();
        foreach (string guid in AssetDatabase.FindAssets("t:AudioClip", new[] { AudioFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            bool isMusic = path.Contains("/Music/");
            var importer = (AudioImporter)AssetImporter.GetAtPath(path);
            AudioImporterSampleSettings s = importer.defaultSampleSettings;
            s.loadType = isMusic ? AudioClipLoadType.CompressedInMemory : AudioClipLoadType.DecompressOnLoad;
            s.compressionFormat = AudioCompressionFormat.Vorbis;
            s.quality = isMusic ? 0.6f : 0.8f;
            importer.defaultSampleSettings = s;
            importer.forceToMono = true;
            importer.SaveAndReimport();
            clips[Path.GetFileNameWithoutExtension(path)] = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
        AudioLibrary lib = AssetDatabase.LoadAssetAtPath<AudioLibrary>(LibraryPath);
        if (lib == null)
        {
            lib = ScriptableObject.CreateInstance<AudioLibrary>();
            AssetDatabase.CreateAsset(lib, LibraryPath);
        }

        lib.mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(AudioFolder + "/MainMixer.mixer");
        lib.menuMusic = clips.TryGetValue("menu_music", out AudioClip menu) ? menu : null;
        lib.levelMusic = clips.TryGetValue("level_music", out AudioClip level) ? level : null;
        lib.jump = Find(clips, "jump");
        lib.fireball = Find(clips, "fireball");
        lib.arrow = Find(clips, "arrow");
        lib.hitPlayer = Find(clips, "hit_player");
        lib.hitEnemy = Find(clips, "hit_enemy");
        lib.pickup = Find(clips, "pickup");
        lib.windupGoblin = Find(clips, "windup_goblin");
        lib.windupEye = Find(clips, "windup_eye");
        lib.windupArcher = Find(clips, "windup_archer");
        lib.windupBoss = Find(clips, "windup_boss");
        lib.deathPlayer = Find(clips, "death_player");
        lib.deathEnemy = Find(clips, "death_enemy");
        lib.uiClick = Find(clips, "ui_click");
        lib.win = Find(clips, "win");
        lib.lose = Find(clips, "lose");
        EditorUtility.SetDirty(lib);

        if (AssetDatabase.LoadAssetAtPath<Sprite>(HeartTarget) == null) AssetDatabase.CopyAsset(HeartSource, HeartTarget);
        AssetDatabase.SaveAssets();

        int missing = 0;
        foreach (Sound sound in System.Enum.GetValues(typeof(Sound)))
        {
            AudioClip[] found = lib.Get(sound);
            if (found == null || found.Length == 0) { Debug.LogWarning("AudioSetup: no clip for " + sound); missing++; }
        }
        Debug.Log($"AudioSetup: {clips.Count} clips imported, mixer {(lib.mixer != null ? "ok" : "MISSING")}, " +
                  $"music {(lib.menuMusic != null && lib.levelMusic != null ? "ok" : "MISSING")}, {missing} sound events missing.");
    }

    static AudioClip[] Find(Dictionary<string, AudioClip> clips, string key)
    {
        var list = new List<AudioClip>();
        var variant = new Regex("^" + Regex.Escape(key) + @"_\d+$");
        foreach (var pair in clips)
            if (pair.Key == key || variant.IsMatch(pair.Key)) list.Add(pair.Value);
        list.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        return list.ToArray();
    }
}
