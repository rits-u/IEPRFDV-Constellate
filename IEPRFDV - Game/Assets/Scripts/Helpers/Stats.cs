using System;
using UnityEngine;

public class Stats : MonoBehaviour 
{
    [SerializeField] private int health = 10;
    [SerializeField] private int attack = 3;

    [Header("UI Elements")]
    [SerializeField] private HealthBar healthBar;

  //  public System.Action OnDamaged;
    public event Action OnDamaged;
    public event Action<Stats> OnDeath;

    public int HP
    {
        get => health;
        set => health = value;
    }
    public int ATK
    {
        get => attack;
        set => attack = value;
    }

    private void Start()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(HP);
        }
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        OnDamaged?.Invoke();
        if (healthBar != null)
        {
            healthBar.SetHealth(HP);
            
        }

        if(HP <= 0)
        {
            //Debug.Log("enemy dead");
            OnDeath?.Invoke(this);
            Destroy(this.gameObject);
        }
        
    }
}
