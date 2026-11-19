using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public static int Tries = 1;
    public static void ResetTries() { Tries = 1; }

    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text triesText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] TMP_Text victoryText;
    [SerializeField] float restartDelay = 1.2f;
    [SerializeField] float victoryDelay = 3f;
    [SerializeField] string menuScene = "MainMenu";

    int score;
    bool levelOver;
    float timeLeft;
    bool timerRunning;
    int levelIndex;
    int levelCount = 1;
    string levelName = "";
    int levelNumber = 1;

    void Awake()
    {
        Instance = this;

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
    }

    void Start()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        UpdateHud();
    }

    public void StartLevel(LevelData data, int index, int count)
    {
        levelIndex = index;
        levelCount = count;
        levelName = data.displayName;
        levelNumber = data.levelNumber;
        timeLeft = data.timeLimit;
        timerRunning = data.timeLimit > 0f;
        UpdateHud();
    }

    void Update()
    {
        if (!timerRunning || levelOver) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            timerRunning = false;
            PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
            if (player != null) player.Kill();
        }
        UpdateTime();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateHud();
    }

    public void PlayerDied()
    {
        if (levelOver) return;
        levelOver = true;
        StartCoroutine(RestartAfterDelay());
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        Tries++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelComplete()
    {
        if (levelOver) return;
        levelOver = true;
        if (victoryPanel != null) victoryPanel.SetActive(true);
        timerRunning = false;
        SaveSystem.CompleteLevel(levelNumber, score, levelCount);
        if (victoryText != null) victoryText.text = $"{levelName} COMPLETE\nTries: {Tries}   Score: {score}\nBest: {SaveSystem.GetLevelBest(levelNumber)}";
        Debug.Log($"{levelName} complete in {Tries} tries");
        StartCoroutine(NextLevel());
    }

    IEnumerator NextLevel()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(victoryDelay);
        Time.timeScale = 1f;
        ResetTries();
        if (levelIndex + 1 < levelCount)
        {
            LevelSelection.Index = levelIndex + 1;
            string nextScene = "Level" + (levelIndex + 2);
            if (Application.CanStreamedLevelBeLoaded(nextScene)) SceneManager.LoadScene(nextScene);
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(menuScene);
        }
    }

    void UpdateHud()
    {
        if (scoreText != null) scoreText.text = $"SCORE {score}";
        if (triesText != null) triesText.text = $"{levelName}   TRY {Tries}".Trim();
        UpdateTime();
    }

    void UpdateTime()
    {
        if (timeText == null) return;
        int seconds = Mathf.CeilToInt(timeLeft);
        timeText.text = $"TIME {seconds / 60}:{seconds % 60:00}";
    }
}
