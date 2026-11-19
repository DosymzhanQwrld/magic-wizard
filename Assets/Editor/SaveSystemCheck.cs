using System;
using System.IO;
using UnityEngine;

public static class SaveSystemCheck
{
    public static void Run()
    {
        string path = SaveSystem.FilePath;
        string backup = path + ".check-backup";
        bool hadFile = File.Exists(path);
        if (hadFile) File.Copy(path, backup, true);

        int passed = 0;
        int failed = 0;
        void Check(string name, bool ok)
        {
            if (ok) passed++; else failed++;
            Debug.Log($"[SaveCheck] {(ok ? "PASS" : "FAIL")} {name}");
        }

        try
        {
            if (File.Exists(path)) File.Delete(path);
            SaveSystem.Load();
            Check("missing file gives defaults", SaveSystem.Data.unlockedLevels.Count == 1 && SaveSystem.IsUnlocked(1) && !SaveSystem.IsUnlocked(2));

            File.WriteAllText(path, "{ this is not json ]]]");
            bool crashed = false;
            try { SaveSystem.Load(); } catch (Exception) { crashed = true; }
            Check("broken file does not crash", !crashed);
            Check("broken file gives defaults", SaveSystem.Data.bestScore == 0 && SaveSystem.IsUnlocked(1) && !SaveSystem.IsUnlocked(2));

            File.WriteAllText(path, "{\"version\":1,\"bestScore\":-50,\"unlockedLevels\":[],\"musicVolume\":7}");
            SaveSystem.Load();
            Check("bad values are fixed", SaveSystem.Data.bestScore == 0 && SaveSystem.IsUnlocked(1) && SaveSystem.Data.musicVolume <= 1f);

            SaveSystem.ResetProgress();
            SaveSystem.CompleteLevel(1, 450, 3);
            SaveSystem.SetMusicVolume(0.3f);
            SaveSystem.Load();
            Check("level 2 unlocked after level 1", SaveSystem.IsUnlocked(2) && !SaveSystem.IsUnlocked(3));
            Check("best score saved", SaveSystem.GetLevelBest(1) == 450 && SaveSystem.Data.bestScore == 450);
            Check("settings saved", Mathf.Approximately(SaveSystem.Data.musicVolume, 0.3f));

            SaveSystem.CompleteLevel(1, 100, 3);
            SaveSystem.Load();
            Check("lower score keeps best", SaveSystem.GetLevelBest(1) == 450);

            SaveSystem.ResetProgress();
            SaveSystem.Load();
            Check("reset clears progress but keeps settings", !SaveSystem.IsUnlocked(2) && SaveSystem.Data.bestScore == 0 && Mathf.Approximately(SaveSystem.Data.musicVolume, 0.3f));
        }
        finally
        {
            if (hadFile) File.Copy(backup, path, true);
            else if (File.Exists(path)) File.Delete(path);
            if (File.Exists(backup)) File.Delete(backup);
            if (File.Exists(path + ".broken")) File.Delete(path + ".broken");
            SaveSystem.Load();
        }

        Debug.Log($"[SaveCheck] {passed} passed, {failed} failed. File: {path}");
    }
}
