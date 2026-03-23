using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;

    public void SetMaxHealth(float health)
    {
        if (!slider) return;
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f);
    }


    //public

    public void SetHealth(float health)
    {
        if (!slider)
        {
            return;
        }
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
