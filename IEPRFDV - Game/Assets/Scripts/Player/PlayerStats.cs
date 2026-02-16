using UnityEngine;

public class PlayerStats : MonoBehaviour 
{
    [SerializeField] private float health = 10;
    [SerializeField] private float attack = 3;

    [Header("UI Elements")]
    [SerializeField] private HealthBar healthBar;

    public System.Action OnDamaged;

    public float HP
    {
        get => health;
        set => health = value;
    }
    public float ATK
    {
        get => attack;
        set => attack = value;
    }

    private void Start()
    {
        healthBar.SetMaxHealth(HP);
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        healthBar.SetHealth(HP);
        OnDamaged?.Invoke();
    }
}
