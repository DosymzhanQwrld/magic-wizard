using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;

    void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.SetPaused(true);
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        AudioManager.SetPaused(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}