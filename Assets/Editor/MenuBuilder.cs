using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class MenuBuilder
{
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "MainMenu")
        {
            Debug.LogError("MenuBuilder: open the MainMenu scene first.");
            return;
        }

        var menu = Object.FindFirstObjectByType<MainMenu>();
        GameObject canvas = GameObject.Find("Canvas");
        if (menu == null || canvas == null) { Debug.LogError("MenuBuilder: MainMenu or Canvas not found."); return; }

        Transform safe = LevelBuilder.FindDeep(canvas.transform, "SafeArea");
        if (safe == null) safe = canvas.transform;
        Transform playButton = LevelBuilder.FindDeep(canvas.transform, "PlayButton");
        Transform quitButton = LevelBuilder.FindDeep(canvas.transform, "QuitButton");
        Transform title = LevelBuilder.FindDeep(canvas.transform, "Title");
        if (playButton == null || quitButton == null) { Debug.LogError("MenuBuilder: Play or Quit button not found."); return; }

        foreach (string old in new[] { "LevelSelectPanel", "SettingsPanel", "ConfirmResetPanel", "SettingsButton", "BestScoreText" })
        {
            Transform t = LevelBuilder.FindDeep(canvas.transform, old);
            if (t != null) Object.DestroyImmediate(t.gameObject);
        }

        Transform mainPanel = LevelBuilder.FindDeep(canvas.transform, "MainPanel");
        if (mainPanel == null)
        {
            mainPanel = Panel("MainPanel", safe, false).transform;
            mainPanel.SetSiblingIndex(0);
        }
        if (title != null) title.SetParent(mainPanel, true);
        playButton.SetParent(mainPanel, true);
        quitButton.SetParent(mainPanel, true);

        Place(playButton, new Vector2(0f, -40f), new Vector2(500f, 130f));
        Rewire(playButton, menu.OpenLevelSelect, "PLAY");
        GameObject settingsButton = CloneButton(playButton, mainPanel, "SettingsButton", "SETTINGS", new Vector2(0f, -190f), new Vector2(500f, 130f));
        Rewire(settingsButton.transform, menu.OpenSettings, "SETTINGS");
        Place(quitButton, new Vector2(0f, -340f), new Vector2(500f, 130f));
        Rewire(quitButton, menu.Quit, "QUIT");
        TextMeshProUGUI best = Text("BestScoreText", mainPanel, "BEST SCORE 0", 40f, new Vector2(0f, 90f), new Vector2(900f, 60f));
        best.color = LevelBuilder.Hex("FAC775");

        GameObject levelPanel = Panel("LevelSelectPanel", safe, true);
        Text("Header", levelPanel.transform, "SELECT LEVEL", 90f, new Vector2(0f, 330f), new Vector2(1200f, 140f)).fontStyle = FontStyles.Bold;
        var levelButtons = new LevelSelectButton[3];
        for (int i = 0; i < 3; i++)
        {
            int number = i + 1;
            GameObject b = CloneButton(playButton, levelPanel.transform, "Level" + number + "Button", "LEVEL " + number,
                new Vector2((i - 1) * 460f, 40f), new Vector2(400f, 300f));
            Button button = b.GetComponent<Button>();
            ClearListeners(button);
            UnityEventTools.AddIntPersistentListener(button.onClick, menu.PlayLevel, number);
            TMP_Text label = b.GetComponentInChildren<TMP_Text>(true);
            label.enableAutoSizing = false;
            label.fontSize = 64f;
            label.richText = true;
            var lsb = b.AddComponent<LevelSelectButton>();
            LevelBuilder.Set(lsb, "levelNumber", number);
            LevelBuilder.Set(lsb, "button", button);
            LevelBuilder.Set(lsb, "label", label);
            levelButtons[i] = lsb;
        }
        GameObject levelBack = CloneButton(playButton, levelPanel.transform, "BackButton", "BACK", new Vector2(0f, -330f), new Vector2(400f, 120f));
        Rewire(levelBack.transform, menu.ShowMain, "BACK");

        GameObject settingsPanel = Panel("SettingsPanel", safe, true);
        Text("Header", settingsPanel.transform, "SETTINGS", 90f, new Vector2(0f, 360f), new Vector2(1200f, 140f)).fontStyle = FontStyles.Bold;
        TextMeshProUGUI musicLabel = Text("MusicValue", settingsPanel.transform, "MUSIC 80%", 48f, new Vector2(0f, 220f), new Vector2(800f, 70f));
        Slider music = MakeSlider("MusicSlider", settingsPanel.transform, new Vector2(0f, 150f));
        TextMeshProUGUI sfxLabel = Text("SfxValue", settingsPanel.transform, "SOUND 100%", 48f, new Vector2(0f, 50f), new Vector2(800f, 70f));
        Slider sfx = MakeSlider("SfxSlider", settingsPanel.transform, new Vector2(0f, -20f));
        GameObject resetButton = CloneButton(playButton, settingsPanel.transform, "ResetButton", "RESET PROGRESS", new Vector2(0f, -160f), new Vector2(600f, 120f));
        Rewire(resetButton.transform, menu.AskReset, "RESET PROGRESS");
        resetButton.GetComponent<Image>().color = new Color(1f, 0.55f, 0.5f);
        GameObject settingsBack = CloneButton(playButton, settingsPanel.transform, "BackButton", "BACK", new Vector2(0f, -300f), new Vector2(400f, 120f));
        Rewire(settingsBack.transform, menu.ShowMain, "BACK");
        TextMeshProUGUI path = Text("SavePathText", settingsPanel.transform, "Save file:", 24f, new Vector2(0f, -400f), new Vector2(1600f, 50f));
        path.color = new Color(1f, 1f, 1f, 0.6f);

        var settings = settingsPanel.AddComponent<SettingsPanel>();
        LevelBuilder.Set(settings, "musicSlider", music);
        LevelBuilder.Set(settings, "sfxSlider", sfx);
        LevelBuilder.Set(settings, "musicValue", musicLabel);
        LevelBuilder.Set(settings, "sfxValue", sfxLabel);
        LevelBuilder.Set(settings, "savePathText", path);
        UnityEventTools.AddPersistentListener(music.onValueChanged, settings.OnMusicChanged);
        UnityEventTools.AddPersistentListener(sfx.onValueChanged, settings.OnSfxChanged);

        GameObject confirm = new GameObject("ConfirmResetPanel", typeof(RectTransform), typeof(Image));
        confirm.transform.SetParent(canvas.transform, false);
        Stretch((RectTransform)confirm.transform);
        confirm.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
        var window = new GameObject("Window", typeof(RectTransform), typeof(Image));
        window.transform.SetParent(confirm.transform, false);
        Place(window.transform, Vector2.zero, new Vector2(1000f, 560f));
        Image windowImage = window.GetComponent<Image>();
        Image playImage = playButton.GetComponent<Image>();
        windowImage.sprite = playImage.sprite;
        windowImage.type = playImage.type;
        Text("Question", window.transform, "RESET ALL PROGRESS?\n<size=60%>Unlocked levels and best scores will be deleted.\nSettings stay.</size>", 56f, new Vector2(0f, 90f), new Vector2(900f, 260f));
        GameObject yes = CloneButton(playButton, window.transform, "YesButton", "YES, RESET", new Vector2(-220f, -170f), new Vector2(380f, 120f));
        Rewire(yes.transform, menu.ConfirmReset, "YES, RESET");
        yes.GetComponent<Image>().color = new Color(1f, 0.55f, 0.5f);
        GameObject no = CloneButton(playButton, window.transform, "NoButton", "NO", new Vector2(220f, -170f), new Vector2(380f, 120f));
        Rewire(no.transform, menu.CancelReset, "NO");
        confirm.SetActive(false);

        LevelBuilder.Set(menu, "mainPanel", mainPanel.gameObject);
        LevelBuilder.Set(menu, "levelSelectPanel", levelPanel);
        LevelBuilder.Set(menu, "settingsPanel", settingsPanel);
        LevelBuilder.Set(menu, "confirmResetPanel", confirm);
        LevelBuilderStep2.SetArray(menu, "levelButtons", levelButtons);
        LevelBuilder.Set(menu, "settings", settings);
        LevelBuilder.Set(menu, "bestScoreText", best);

        levelPanel.SetActive(false);
        settingsPanel.SetActive(false);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("MenuBuilder: level select, settings and reset panels are ready.");
    }

    static GameObject Panel(string name, Transform parent, bool dark)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Stretch((RectTransform)go.transform);
        if (dark)
        {
            var image = go.AddComponent<Image>();
            image.color = new Color(0.05f, 0.02f, 0.08f, 0.55f);
        }
        return go;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void Place(Transform t, Vector2 pos, Vector2 size)
    {
        var rt = (RectTransform)t;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static TextMeshProUGUI Text(string name, Transform parent, string text, float size, Vector2 pos, Vector2 box)
    {
        TextMeshProUGUI t = LevelBuilderStep2.NewText(name, parent, text, size);
        t.richText = true;
        Place(t.transform, pos, box);
        return t;
    }

    static GameObject CloneButton(Transform template, Transform parent, string name, string label, Vector2 pos, Vector2 size)
    {
        GameObject copy = Object.Instantiate(template.gameObject, parent);
        copy.name = name;
        Place(copy.transform, pos, size);
        TMP_Text text = copy.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
        {
            text.text = label;
            text.enableAutoSizing = true;
            text.fontSizeMin = 28f;
            text.fontSizeMax = 64f;
            var textRect = (RectTransform)text.transform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(30f, 12f);
            textRect.offsetMax = new Vector2(-30f, -12f);
        }
        return copy;
    }

    static void Rewire(Transform buttonTransform, UnityAction action, string label)
    {
        Button button = buttonTransform.GetComponent<Button>();
        ClearListeners(button);
        UnityEventTools.AddPersistentListener(button.onClick, action);
        TMP_Text text = buttonTransform.GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = label;
    }

    static void ClearListeners(Button button)
    {
        for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            UnityEventTools.RemovePersistentListener(button.onClick, i);
    }

    static Slider MakeSlider(string name, Transform parent, Vector2 pos)
    {
        GameObject go = DefaultControls.CreateSlider(new DefaultControls.Resources());
        go.name = name;
        go.transform.SetParent(parent, false);
        Place(go.transform, pos, new Vector2(800f, 50f));
        Slider slider = go.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;

        foreach (Image image in go.GetComponentsInChildren<Image>(true))
        {
            if (image.name == "Background") image.color = new Color(0.15f, 0.08f, 0.2f, 0.9f);
            else if (image.name == "Fill") image.color = LevelBuilder.Hex("EF9F27");
            else if (image.name == "Handle") image.color = LevelBuilder.Hex("FFE9C4");
        }
        Transform handle = LevelBuilder.FindDeep(go.transform, "Handle");
        if (handle != null) ((RectTransform)handle).sizeDelta = new Vector2(60f, 0f);
        return slider;
    }
}
