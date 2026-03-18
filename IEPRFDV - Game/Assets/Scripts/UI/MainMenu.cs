using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private GameObject settings;


    string gameScene = "GameScene";
    private void Start()
    {
        sceneTransition.AsyncLoadScene(gameScene);
        settings.SetActive(false);
    }

    public void StartGame()
    {
        sceneTransition.LoadScene(gameScene);
    }

    public void SetSettings(bool value)
    {
        settings.SetActive(value);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
