using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;

    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    private void Start()
    {
        GetComponent<PlayerStats>().OnDamaged += Flash;
    }

    public void Flash()
    {
        StopAllCoroutines(); 
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float t = 0;

        while (t < flashDuration)
        {
            sr.color = Color.Lerp(flashColor, originalColor, t / flashDuration);
            t += Time.deltaTime;
            yield return null;
        }

        sr.color = originalColor;
    }
}
