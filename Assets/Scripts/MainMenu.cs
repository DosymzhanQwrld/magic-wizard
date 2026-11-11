using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        GameSession.ResetTries();
        SceneManager.LoadScene("Level1");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}