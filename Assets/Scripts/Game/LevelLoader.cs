using UnityEngine;

[ExecuteAlways]
public class LevelLoader : MonoBehaviour
{
    const string PreviewName = "Level Preview (not saved)";

    [SerializeField] LevelData[] levels;
    [SerializeField] int sceneLevel = -1;
    [SerializeField] int editorTestLevel = -1;
    [SerializeField] bool showPreviewInEditor = true;

    [Header("Scene")]
    [SerializeField] Transform player;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] Transform cameraBackground;
    [SerializeField] Vector2 backgroundSize = new Vector2(34f, 18f);

    [Header("Prefabs")]
    [SerializeField] GoblinEnemy goblinPrefab;
    [SerializeField] FlyingEyeEnemy flyingEyePrefab;
    [SerializeField] ArcherEnemy archerPrefab;
    [SerializeField] LevelGoal portalPrefab;
    [SerializeField] Hazard lavaPrefab;
    [SerializeField] BossEnemy bossPrefab;
    [SerializeField] BossHealthBar bossHealthBar;

    [Header("Ground look")]
    [SerializeField] Sprite grassSprite;
    [SerializeField] Sprite dirtSprite;
    [SerializeField] float grassHeight = 0.5f;

    public LevelData Current { get; private set; }
    public int CurrentIndex { get; private set; }
    public int LevelCount => levels.Length;

    Transform root;
    LevelData building;

    void Awake()
    {
        if (!Application.isPlaying) return;

        int index = sceneLevel >= 0 ? sceneLevel : LevelSelection.Index;
#if UNITY_EDITOR
        ClearPreview();
        if (editorTestLevel >= 0) index = editorTestLevel;
#endif
        CurrentIndex = Mathf.Clamp(index, 0, levels.Length - 1);
        LevelSelection.Index = CurrentIndex;
        Current = levels[CurrentIndex];

        root = new GameObject("Level " + Current.levelNumber).transform;
        Build(Current, false);

        if (player != null) player.position = OnGround(Current.playerSpawn, player);
        if (cameraFollow != null) cameraFollow.SetBounds(Current.cameraMinX, Current.cameraMaxX);
    }

    void Start()
    {
        if (!Application.isPlaying) return;
        if (GameSession.Instance != null) GameSession.Instance.StartLevel(Current, CurrentIndex, levels.Length);
    }

    void Build(LevelData data, bool preview)
    {
        building = data;
        ApplyBackground(data, preview);
        int groundLayer = LayerMask.NameToLayer("Ground");
        foreach (BlockData block in data.blocks) BuildBlock(block, groundLayer);
        foreach (BlockData lava in data.lava) BuildLava(lava);
        foreach (EnemySpawn spawn in data.enemies) SpawnEnemy(spawn);

        if (!preview && bossHealthBar != null) bossHealthBar.Hide();
        if (data.hasBoss && bossPrefab != null)
        {
            BossEnemy boss = Instantiate(bossPrefab, data.bossPosition, Quaternion.identity, root);
            if (!preview) boss.Setup(data.bossLeftX, data.bossRightX, data.bossActivateX, bossHealthBar);
        }
        else if (portalPrefab != null)
        {
            Instantiate(portalPrefab, OnGround(data.goalPosition, portalPrefab), Quaternion.identity, root);
        }
    }

    void BuildBlock(BlockData block, int groundLayer)
    {
        var go = new GameObject("Ground");
        go.layer = groundLayer;
        go.transform.SetParent(root, false);
        go.transform.position = block.center;
        go.AddComponent<BoxCollider2D>().size = block.size;

        Sprite fill = building != null && building.groundFillSprite != null ? building.groundFillSprite : dirtSprite;
        Sprite topSprite = building != null && building.groundTopSprite != null ? building.groundTopSprite : grassSprite;
        float topHeight = building != null && building.groundTopHeight > 0f ? building.groundTopHeight : grassHeight;
        topHeight = Mathf.Min(topHeight, block.size.y);

        AddTiled(go.transform, "Dirt", fill, Vector2.zero, block.size, 0);
        float top = block.size.y * 0.5f - topHeight * 0.5f;
        AddTiled(go.transform, "Top", topSprite, new Vector2(0f, top), new Vector2(block.size.x, topHeight), 1);
    }

    void ApplyBackground(LevelData data, bool preview)
    {
        if (data.backgroundSprite == null) return;

        Camera cam = Camera.main;
        if (!preview)
        {
            if (cam != null) cam.backgroundColor = data.skyColor;
            if (cameraBackground != null)
                foreach (Transform layer in cameraBackground) layer.gameObject.SetActive(false);
        }

        var go = new GameObject("Level Background");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = data.backgroundSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = backgroundSize;
        sr.sortingOrder = -40;

        Transform parent = !preview && cam != null ? (cameraBackground != null ? cameraBackground : cam.transform) : root;
        go.transform.SetParent(parent, false);
        if (parent == root)
        {
            float x = data.playerSpawn.x + backgroundSize.x * 0.25f;
            go.transform.position = new Vector3(Mathf.Clamp(x, data.cameraMinX, data.cameraMaxX), 3.5f, 10f);
        }
        else
        {
            go.transform.localPosition = new Vector3(0f, 0f, parent == cameraBackground ? 0f : 10f);
        }
    }

    void AddTiled(Transform parent, string name, Sprite sprite, Vector2 localPos, Vector2 size, int order)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = size;
        sr.sortingOrder = order;
    }

    void BuildLava(BlockData lava)
    {
        if (lavaPrefab == null) return;
        Hazard hazard = Instantiate(lavaPrefab, lava.center, Quaternion.identity, root);
        hazard.transform.localScale = new Vector3(lava.size.x, lava.size.y, 1f);
    }

    void SpawnEnemy(EnemySpawn spawn)
    {
        switch (spawn.type)
        {
            case EnemyType.Goblin:
                if (goblinPrefab == null) return;
                GoblinEnemy goblin = Instantiate(goblinPrefab, OnGround(spawn.position, goblinPrefab), Quaternion.identity, root);
                goblin.SetPatrol(MakePoint("Patrol A", spawn.patrolLeft, goblin.transform.position.y),
                                 MakePoint("Patrol B", spawn.patrolRight, goblin.transform.position.y));
                break;

            case EnemyType.FlyingEye:
                if (flyingEyePrefab == null) return;
                Instantiate(flyingEyePrefab, spawn.position, Quaternion.identity, root);
                break;

            case EnemyType.Archer:
                if (archerPrefab == null) return;
                ArcherEnemy archer = Instantiate(archerPrefab, OnGround(spawn.position, archerPrefab), Quaternion.identity, root);
                archer.SetStartFacing(spawn.facing == 0 ? -1 : spawn.facing);
                break;
        }
    }

    Transform MakePoint(string name, float x, float y)
    {
        var point = new GameObject(name).transform;
        point.SetParent(root, false);
        point.position = new Vector3(x, y, 0f);
        return point;
    }

    static Vector3 OnGround(Vector2 feet, Component thing)
    {
        return new Vector3(feet.x, feet.y + HalfHeight(thing) + 0.02f, 0f);
    }

    static float HalfHeight(Component thing)
    {
        float scaleY = Mathf.Abs(thing.transform.localScale.y);
        if (thing.TryGetComponent(out BoxCollider2D box)) return box.size.y * scaleY * 0.5f;
        if (thing.TryGetComponent(out CircleCollider2D circle)) return circle.radius * scaleY;
        return 0.5f * scaleY;
    }

#if UNITY_EDITOR
    GameObject preview;

    void OnEnable()
    {
        if (!Application.isPlaying) UnityEditor.EditorApplication.delayCall += RefreshPreview;
    }

    void OnDisable()
    {
        ClearPreview();
    }

    void OnValidate()
    {
        if (!Application.isPlaying) UnityEditor.EditorApplication.delayCall += RefreshPreview;
    }

    void RefreshPreview()
    {
        if (this == null || Application.isPlaying || !isActiveAndEnabled) return;
        ClearPreview();
        if (!showPreviewInEditor || levels == null || levels.Length == 0) return;

        int index = editorTestLevel >= 0 ? editorTestLevel : (sceneLevel >= 0 ? sceneLevel : 0);
        LevelData data = levels[Mathf.Clamp(index, 0, levels.Length - 1)];
        if (data == null) return;

        root = new GameObject(PreviewName).transform;
        Build(data, true);
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            t.gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
        preview = root.gameObject;
        root = null;
    }

    void ClearPreview()
    {
        if (preview != null) DestroyImmediate(preview);
        preview = null;
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            if (go != null && go.name == PreviewName && go.transform.parent == null && (go.hideFlags & HideFlags.DontSave) != 0)
                DestroyImmediate(go);
        var scene = gameObject.scene;
        if (!scene.IsValid() || !scene.isLoaded) return;
        foreach (GameObject go in scene.GetRootGameObjects())
            if (go.name == PreviewName) DestroyImmediate(go);
    }
#endif
}
