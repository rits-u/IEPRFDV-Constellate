using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : ScreenUI
{
    [Header("UI Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmRestartPanel;
    [SerializeField] private GameObject confirmQuitPanel;

    [Header("Volume")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgmSlider;

    private void Start()
    {
        screenName = "Pause";

        sfxSlider.maxValue = AudioManager.Instance.GetSFXSource().volume;
        bgmSlider.maxValue = AudioManager.Instance.GetBGMSource().volume;
    }

    public override void ShowScreenUI()
    {
        gameObject.SetActive(true);
        PauseGame();
    }

    public override void HideScreenUI() 
    {
        gameObject.SetActive(false);
        ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        LeanTween.pauseAll();
        PlayerManager.Instance.DisableAllPlayerMovement();

        foreach (var enemy in FindObjectsByType<AI_Controller>(FindObjectsSortMode.None))
        {
            enemy.SetPaused(true); 
        }

        AudioManager.Instance.SetPaused(true);

        Debug.Log("Game is Paused");
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        LeanTween.resumeAll();
        PlayerManager.Instance.EnableAllPlayerMovement();

        foreach (var enemy in FindObjectsByType<AI_Controller>(FindObjectsSortMode.None))
        {
            enemy.SetPaused(false);
        }

        AudioManager.Instance.SetPaused(false);

        Debug.Log("Game is Resumed");
    }

    public void OnResumeButtonPressed()
    {
        UIManager.Instance.isPaused = false;
        UIManager.Instance.CloseScreen("Pause");
    }

    //SETTINGS
    public void OnSettingsButtonPressed()
    {
        settingsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void OnBGMVolumeChange()
    {
        AudioManager.Instance.GetBGMSource().volume = bgmSlider.value;
    }

    public void OnSFXVolumeChange()
    {
        AudioManager.Instance.GetSFXSource().volume = sfxSlider.value;
    }

    //RESTART
    public void OnRestartButtonPressed()
    {
        confirmRestartPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void OnConfirmRestartPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //QUIT
    public void OnQuitButtonPressed()
    {
        confirmQuitPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void OnConfirmQuitPressed()
    {
        Application.Quit();
    }

    public void OnBackButtonPressed()
    {
        settingsPanel.SetActive(false);
        confirmRestartPanel.SetActive(false);
        confirmQuitPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
