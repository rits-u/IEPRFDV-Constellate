using System;
using System.Collections;
using System.Drawing;
using UnityEngine;

public class Stats : MonoBehaviour 
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 10;
    [SerializeField] private float health = 10;
    [SerializeField] private int attack = 3;
    [SerializeField] private int shield;

   // [Header("Bound")]
   // [SerializeField] private int maxHealth = 10;
  

    [Header("UI Elements")]
    [SerializeField] private HealthBar healthBar;

  //  public System.Action OnDamaged;
    public event Action OnDamaged;
    public event Action<Stats> OnDeath;

    public bool isDown;

    public float MaxHP
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    public float HP
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
        isDown = false;
        //UpdateMaximumValues();
        UpdateBar();
    }

    public bool TakeDamage(int damage)
    {
       // ValidateStats();

        if (SP > 0)
        {
            SP -= damage;
            UpdateSP();
        }
        else
        {
            HP -= damage;
            UpdateHP();
        }
        ValidateStats();
        OnDamaged?.Invoke();


        if(HP <= 0)
        {
            OnDeath?.Invoke(this);
     //       Destroy(this.gameObject);
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

    public void UpdateMaximumValues()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(MaxHP);
            healthBar.SetMaxShield(SP);
        }
    }

    public void UpdateBar()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(MaxHP);
            healthBar.SetMaxShield(SP);

            healthBar.SetShield(SP);
            healthBar.SetHealth(HP);
        }
    }

    public void UpdateHP()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(HP);
        }
    }
    public void UpdateMaxAndHP()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(MaxHP);
            healthBar.SetHealth(HP);
        }
    }


    public void UpdateSP()
    {
        if (healthBar != null)
        {
            healthBar.SetShield(SP);
        }
    }

    public void MakePlayerInvulnerable(float duration)
    {
        GetComponent<DamageFlash>().enabled = true;
        GetComponent<DamageFlash>().InvulnerableState();
        StartCoroutine(PlayerInvulnerable(duration));
    }

    private IEnumerator PlayerInvulnerable(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDown = false;
        GetComponent<DamageFlash>().ResetSpritesColor();
    }
}
