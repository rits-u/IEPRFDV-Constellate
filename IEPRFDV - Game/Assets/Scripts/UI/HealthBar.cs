using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private Image healthFill;

    [Header("Shield")]
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private Gradient shieldGradient;
    [SerializeField] private Image shieldFill;

    public void SetMaxHealth(float health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
        healthFill.color = healthGradient.Evaluate(1f);
    }

    public void SetMaxShield(float shield)
    {
        if (shieldSlider != null)
        {
            shieldSlider.maxValue = shield;
            shieldSlider.value = shield;
            shieldFill.color = shieldGradient.Evaluate(1f);
        }
    }

    public void SetHealth(float health)
    {
        healthSlider.value = health;
        healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
    }

    public void SetShield(float shield)
    {
        if (shieldSlider != null)
        {
            shieldSlider.value = shield;
            shieldFill.color = shieldGradient.Evaluate(shieldSlider.normalizedValue);
        }
    }

}
