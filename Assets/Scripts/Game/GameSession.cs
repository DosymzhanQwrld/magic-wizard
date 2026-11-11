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
    [SerializeField] GameObject victoryPanel;
    [SerializeField] TMP_Text victoryText;
    [SerializeField] float restartDelay = 1.2f;
    [SerializeField] float victoryDelay = 3f;
    [SerializeField] string menuScene = "MainMenu";

    int score;
    bool levelOver;

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
        if (victoryText != null) victoryText.text = $"LEVEL COMPLETE\nTries: {Tries}   Score: {score}";
        Debug.Log($"Level complete in {Tries} tries");
        StartCoroutine(BackToMenu());
    }

    IEnumerator BackToMenu()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(victoryDelay);
        Time.timeScale = 1f;
        ResetTries();
        SceneManager.LoadScene(menuScene);
    }

    void UpdateHud()
    {
        if (scoreText != null) scoreText.text = $"SCORE {score}";
        if (triesText != null) triesText.text = $"TRY {Tries}";
    }
}
