using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class LevelBuilderStep6
{
    [MenuItem("Magic Wizard/Step 6 - Portal and Victory")]
    public static void Step6()
    {
        if (!LevelBuilder.CheckScene()) return;

        var session = Object.FindFirstObjectByType<GameSession>();
        if (session == null)
        {
            EditorUtility.DisplayDialog("Magic Wizard", "No GameSession in the scene. Run Step 2 first.", "OK");
            return;
        }

        LevelBuilder.Remove("Portal");
        GameObject level = GameObject.Find("Level");
        Sprite circle = LevelBuilder.MakeSprite("Circle", true);
        GameObject portal = LevelBuilder.Sprite2D("Portal", circle, LevelBuilder.Hex("5DCAA5"),
            new Vector2(62.5f, 1f), new Vector2(1.2f, 2f), level != null ? level.transform : null, 0);
        portal.AddComponent<BoxCollider2D>().isTrigger = true;
        portal.AddComponent<LevelGoal>();

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null) { Debug.LogWarning("No Canvas found."); return; }

        Transform victory = LevelBuilder.FindDeep(canvas.transform, "VictoryPanel");
        if (victory == null)
        {
            var panel = new GameObject("VictoryPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(canvas.transform, false);
            var rt = (RectTransform)panel.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f);

            TextMeshProUGUI text = LevelBuilderStep2.NewText("VictoryText", panel.transform, "LEVEL COMPLETE", 90f);
            text.color = LevelBuilder.Hex("FAC775");
            RectTransform trt = text.rectTransform;
            trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 0.5f);
            trt.sizeDelta = new Vector2(1400f, 400f);

            Transform pause = LevelBuilder.FindDeep(canvas.transform, "PausePanel");
            if (pause != null) panel.transform.SetSiblingIndex(pause.GetSiblingIndex());
            victory = panel.transform;
        }

        LevelBuilder.Set(session, "victoryPanel", victory.gameObject);
        LevelBuilder.Set(session, "victoryText", LevelBuilder.FindDeep(victory, "VictoryText").GetComponent<TMP_Text>());

        LevelBuilder.Finish("Step 6 done: portal and victory screen are ready.");
    }
}
