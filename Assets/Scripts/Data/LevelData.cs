using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Goblin, FlyingEye, Archer }

[Serializable]
public struct BlockData
{
    public Vector2 center;
    public Vector2 size;
}

[Serializable]
public struct EnemySpawn
{
    public EnemyType type;
    public Vector2 position;
    public float patrolLeft;
    public float patrolRight;
    public int facing;
}

[CreateAssetMenu(fileName = "Level", menuName = "Magic Wizard/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public string displayName;
    public float timeLimit;

    [Header("Positions")]
    public Vector2 playerSpawn;
    public Vector2 goalPosition;
    public float cameraMinX;
    public float cameraMaxX;

    [Header("Look")]
    public Sprite groundTopSprite;
    public Sprite groundFillSprite;
    public float groundTopHeight;
    public Sprite backgroundSprite;
    public Color skyColor = Color.black;

    [Header("Layout")]
    public List<BlockData> blocks = new List<BlockData>();
    public List<BlockData> lava = new List<BlockData>();

    [Header("Enemies")]
    public List<EnemySpawn> enemies = new List<EnemySpawn>();

    [Header("Boss")]
    public bool hasBoss;
    public Vector2 bossPosition;
    public float bossLeftX;
    public float bossRightX;
    public float bossActivateX;
}
