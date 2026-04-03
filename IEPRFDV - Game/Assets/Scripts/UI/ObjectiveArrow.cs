using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ObjectiveArrow : MonoBehaviour
{
    [SerializeField] private Transform objective;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private GameObject arrow;
    [SerializeField] private float edgeAllowance = 50f;

    private RectTransform objectiveImage;
    private UnityEngine.UI.Image image;
    private Coroutine coroutine;
    bool showArrow = false;

    private void Awake()
    {
        objectiveImage = arrow.GetComponent<RectTransform>();
        image = arrow.GetComponent<UnityEngine.UI.Image>();
        gameObject.SetActive(true);
    }
    public void ToggleShowArrow(bool value)
    {
        showArrow = value;
        Color color = image.color;
        color.a = value ? 1 : 0;
        image.color = color;
        if (value)
        {
            if (coroutine == null)
            {
                coroutine = StartCoroutine(ShowArrow());
            }
        }
        else
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
        
        Debug.Log("Value: " + value);
    }

    IEnumerator ShowArrow()
    {
        if (objective == null || gameCamera == null) yield break;
        if (objectiveImage == null)
        {
            Debug.LogError($"{this.gameObject.name} obj image null");
            yield break;
        }
        if (!objectiveImage)
        {
            Debug.LogError($"{this.gameObject.name} (2)obj image null");
            yield break;
        }

        while (showArrow)
        {
            Vector3 screenPos = gameCamera.WorldToScreenPoint(objective.position);

            screenPos.x = Mathf.Clamp(screenPos.x, edgeAllowance, Screen.width - edgeAllowance);
            screenPos.y = Mathf.Clamp(screenPos.y, edgeAllowance, Screen.height - edgeAllowance);

            objectiveImage.position = screenPos;

            Vector3 direction = (objective.position - gameCamera.transform.position).normalized;
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            objectiveImage.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
    }
}
