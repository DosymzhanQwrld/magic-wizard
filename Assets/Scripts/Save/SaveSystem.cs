using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    const string FileName = "save.json";

    static SaveData data;

    public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static SaveData Data
    {
        get
        {
            if (data == null) Load();
            return data;
        }
    }

    public static void Load()
    {
        data = ReadFile();
        Validate(data);
    }

    public static void Save()
    {
        if (data == null) return;
        string temp = FilePath + ".tmp";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(temp, JsonUtility.ToJson(data, true));
            if (File.Exists(FilePath)) File.Delete(FilePath);
            File.Move(temp, FilePath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Save failed: " + e.Message);
        }
    }

    public static bool IsUnlocked(int levelNumber)
    {
        return levelNumber <= 1 || Data.unlockedLevels.Contains(levelNumber);
    }

    public static int GetLevelBest(int levelNumber)
    {
        int i = levelNumber - 1;
        return i >= 0 && i < Data.levelBestScores.Count ? Data.levelBestScores[i] : 0;
    }

    public static void CompleteLevel(int levelNumber, int score, int levelCount)
    {
        SaveData d = Data;
        while (d.levelBestScores.Count < levelNumber) d.levelBestScores.Add(0);
        if (score > d.levelBestScores[levelNumber - 1]) d.levelBestScores[levelNumber - 1] = score;
        if (score > d.bestScore) d.bestScore = score;

        int next = levelNumber + 1;
        if (next <= levelCount && !d.unlockedLevels.Contains(next)) d.unlockedLevels.Add(next);
        d.unlockedLevels.Sort();
        Save();
    }

    public static void SetMusicVolume(float value)
    {
        Data.musicVolume = Mathf.Clamp01(value);
        Save();
    }

    public static void SetSfxVolume(float value)
    {
        Data.sfxVolume = Mathf.Clamp01(value);
        Save();
    }

    public static void ResetProgress()
    {
        var fresh = new SaveData
        {
            musicVolume = Data.musicVolume,
            sfxVolume = Data.sfxVolume
        };
        data = fresh;
        Save();
    }

    static SaveData ReadFile()
    {
        if (!File.Exists(FilePath)) return new SaveData();
        try
        {
            string json = File.ReadAllText(FilePath);
            SaveData loaded = JsonUtility.FromJson<SaveData>(json);
            if (loaded == null) throw new FormatException("empty save");
            return loaded;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Save file is broken, using defaults ({e.Message})");
            try { File.Copy(FilePath, FilePath + ".broken", true); } catch (Exception) { }
            return new SaveData();
        }
    }

    static void Validate(SaveData d)
    {
        if (d.unlockedLevels == null) d.unlockedLevels = new System.Collections.Generic.List<int>();
        if (!d.unlockedLevels.Contains(1)) d.unlockedLevels.Add(1);
        d.unlockedLevels.RemoveAll(level => level < 1);
        d.unlockedLevels.Sort();
        if (d.levelBestScores == null) d.levelBestScores = new System.Collections.Generic.List<int>();
        for (int i = 0; i < d.levelBestScores.Count; i++) d.levelBestScores[i] = Mathf.Max(0, d.levelBestScores[i]);
        d.bestScore = Mathf.Max(0, d.bestScore);
        d.musicVolume = float.IsNaN(d.musicVolume) ? new SaveData().musicVolume : Mathf.Clamp01(d.musicVolume);
        d.sfxVolume = float.IsNaN(d.sfxVolume) ? new SaveData().sfxVolume : Mathf.Clamp01(d.sfxVolume);
        d.version = SaveData.CurrentVersion;
    }
}
