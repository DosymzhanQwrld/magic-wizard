using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public const int CurrentVersion = 1;

    public int version = CurrentVersion;
    public int bestScore;
    public List<int> unlockedLevels = new List<int> { 1 };
    public List<int> levelBestScores = new List<int>();
    public float musicVolume = 0.8f;
    public float sfxVolume = 1f;
}
