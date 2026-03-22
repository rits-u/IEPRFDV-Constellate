using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [Header("Properties")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    
    AsyncOperation asyncOperation;
    public bool isLoading => asyncOperation != null && !asyncOperation.isDone;

    private void Awake()
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;
    }

    public IEnumerator AsyncLoadScene(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' cannot be loaded.");
            yield break;
        }
        if (asyncOperation != null && !asyncOperation.isDone)
        {
            Debug.LogWarning($"A scene is already loading");
            yield break;
        }

        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        while(asyncOperation.progress < .9f)
        {
            yield return null;
        }
        asyncOperation.allowSceneActivation = true;
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeLoadScene(sceneName));
        
    }

    IEnumerator FadeLoadScene(string sceneName)
    {
        float time = 0f;
        Color color = fadeImage.color;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = time / fadeDuration;
            fadeImage.color = color;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}
