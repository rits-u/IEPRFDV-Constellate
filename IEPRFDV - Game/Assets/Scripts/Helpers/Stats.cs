using System;
using UnityEngine;

public class Stats : MonoBehaviour 
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int health = 10;
    [SerializeField] private int attack = 3;
    [SerializeField] private int shield;

   // [Header("Bound")]
   // [SerializeField] private int maxHealth = 10;

    [Header("UI Elements")]
    [SerializeField] private HealthBar healthBar;

  //  public System.Action OnDamaged;
    public event Action OnDamaged;
    public event Action<Stats> OnDeath;

    public int MaxHP
    {
        get => maxHealth;
        set => maxHealth = value;
    }

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

    public int SP
    {
        get => shield;
        set => shield = value;
    }

    public void ValidateStats()
    {
        if(HP > MaxHP) HP = MaxHP;  //unequipping

        if(HP < 0) HP = 0;  //damage
            
        if(SP < 0) SP = 0;  //damage

        if(ATK <= 0) ATK = 1;   //unequipping
    }

    private void Start()
    {
        health = maxHealth;

        UpdateHealthBar();
    }

    public bool TakeDamage(int damage)
    {
       // ValidateStats();

        if (SP > 0)
        {
            SP -= damage;
            if(healthBar != null)
            {
                healthBar.SetShield(SP);
            }
        }
        else
        {
            HP -= damage;
            if (healthBar != null)
            {
                healthBar.SetHealth(HP);

            }
        }
        ValidateStats();
        OnDamaged?.Invoke();

        //if (healthBar != null)
        //{
        //    healthBar.SetHealth(HP);
            
        //}

        if(HP <= 0)
        {
            //Debug.Log("enemy dead");
            OnDeath?.Invoke(this);
            Destroy(this.gameObject);
            return true;
        }

        return false;
    }

    public void Heal(int healAmount)
    {
        HP += healAmount;
        if(HP > maxHealth)
        {
            HP = maxHealth;
        }
    }

    public void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(MaxHP);
            healthBar.SetMaxShield(SP);
        }
    }
}
