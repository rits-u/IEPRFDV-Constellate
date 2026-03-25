using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    private SpriteRenderer[] renderers;
    private Color[] originalColors;

    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.3f;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();

        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        GetComponent<Stats>().OnDamaged += Flash;
    }

    private void OnDisable()
    {
        GetComponent<Stats>().OnDamaged -= Flash;
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].color = flashColor;

        Animator animator = GetComponent<Animator>();
        if (animator != null) animator.enabled = false;

        yield return new WaitForSeconds(flashDuration);

        if (animator != null) animator.enabled = true;

        ResetSpritesColor();
    }

    public void InvulnerableState()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Color color = originalColors[i];
            color.a = 67f/255f;
            renderers[i].color = color;
        }
    }

    public void ResetSpritesColor()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].color = originalColors[i];
    }
}