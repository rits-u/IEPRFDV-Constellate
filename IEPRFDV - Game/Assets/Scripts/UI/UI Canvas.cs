using UnityEditor.PackageManager.UI;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject settingsScreen;

    bool isPaused = false;

    public void ResumeGame()
    {
        settingsScreen.SetActive(false);
        pauseScreen.SetActive(false);
        isPaused = false;
    }
    public void SetPauseScreen(bool value)
    {
        isPaused = true;
        pauseScreen.SetActive(value);
    }
    public void SetSettingsScreen(bool value)
    {
        pauseScreen.SetActive(!value);
        settingsScreen.SetActive(value);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public bool GetIsPaused() { return isPaused; }
}
