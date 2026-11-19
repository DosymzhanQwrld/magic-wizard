using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject levelSelectPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject confirmResetPanel;
    [SerializeField] LevelSelectButton[] levelButtons;
    [SerializeField] SettingsPanel settings;
    [SerializeField] TMP_Text bestScoreText;

    void Start()
    {
        ShowMain();
    }

    public void Play()
    {
        OpenLevelSelect();
    }

    public void PlayLevel(int levelNumber)
    {
        if (!SaveSystem.IsUnlocked(levelNumber)) return;
        GameSession.ResetTries();
        LevelSelection.Index = levelNumber - 1;
        SceneManager.LoadScene("Level" + levelNumber);
    }

    public void ShowMain()
    {
        Show(mainPanel);
        if (bestScoreText != null) bestScoreText.text = $"BEST SCORE {SaveSystem.Data.bestScore}";
    }

    public void OpenLevelSelect()
    {
        Show(levelSelectPanel);
        if (levelButtons == null) return;
        foreach (LevelSelectButton b in levelButtons)
            if (b != null) b.Refresh();
    }

    public void OpenSettings()
    {
        Show(settingsPanel);
        if (settings != null) settings.Refresh();
    }

    public void AskReset()
    {
        if (confirmResetPanel != null) confirmResetPanel.SetActive(true);
    }

    public void CancelReset()
    {
        if (confirmResetPanel != null) confirmResetPanel.SetActive(false);
    }

    public void ConfirmReset()
    {
        SaveSystem.ResetProgress();
        CancelReset();
        ShowMain();
    }

    public void Quit()
    {
        SaveSystem.Save();
        Debug.Log("Quit");
        Application.Quit();
    }

    void Show(GameObject panel)
    {
        if (mainPanel != null) mainPanel.SetActive(panel == mainPanel);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(panel == levelSelectPanel);
        if (settingsPanel != null) settingsPanel.SetActive(panel == settingsPanel);
        if (confirmResetPanel != null) confirmResetPanel.SetActive(false);
    }
}
