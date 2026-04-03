using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ResurrectCircle : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float colRadius = 8f;
    [SerializeField] private float resCheck = 0.5f;
    [SerializeField] private float healInterval = 1.0f;
    [SerializeField] private float hpHealPercent = 0.15f;
    [SerializeField] private float hpThreshold = 0.60f;

    [Header("Colors")]
    [SerializeField] private Color stateColor = Color.blue;
    [SerializeField] private Color healStateColor = Color.green;

    [Header("Visuals")]
    [SerializeField] private Sprite resSprite;
    [SerializeField] private Sprite healSprite;


    private Color[] originalColors;


    private CircleCollider2D col;
    private Player player;
    private Stats playerStats;
    private bool isActivated;
    private bool isBeingResurrected;
    private bool isDoneHealing;

    private float timeUpdate = 0f;
    private float healTimeUpdate = 0f;

    private SpriteRenderer sr;

    private void OnEnable()
    {
        player = GetComponentInParent<Player>();
        playerStats = player.GetComponent<Stats>();
        sr = GetComponent<SpriteRenderer>();    

        playerStats.OnDeath += ActivateCircle;
    }

    private void OnDisable()
    {
        playerStats.OnDeath -= ActivateCircle;
    }

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
     // col.radius = colRadius;

        SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer>();

        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].color; 
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isActivated) return;

        if(other.CompareTag("Player") && !isBeingResurrected)
        {
            timeUpdate += Time.deltaTime;

            if(timeUpdate >= resCheck)
            {
                isBeingResurrected = true;
                timeUpdate = 0f;
            } 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isBeingResurrected && !isDoneHealing)
        {
            isBeingResurrected = false;
            DownState();
            
        }
    }

    private void Update()
    {
        if(isBeingResurrected)
        {
            HealingState();
            healTimeUpdate += Time.deltaTime;

            if(healTimeUpdate >= healInterval) 
            {
                float increment = playerStats.MaxHP * hpHealPercent;

                playerStats.HP += increment;
                playerStats.ValidateStats();
                playerStats.UpdateHP();

              //  Debug.Log($"Heal Player! HP: {playerStats.HP}");
                healTimeUpdate = 0f;

                if (playerStats.HP >= playerStats.MaxHP * hpThreshold)
                {
                    isDoneHealing = true;

                    DeactivateCircle();
                }
            }
        }
    }

    //activates when player dies from .onDeath in stats
    private void ActivateCircle(Stats stats)
    {
        isActivated = true;
        sr.enabled = true;
        col.enabled = true;
        isDoneHealing = false;

        DownState();
        PlayerManager.Instance.PutPlayerToDownState(player);
       // Debug.Log($"{player.Name}'s res circle was activated");
    }

    private void DeactivateCircle()
    {
        sr.enabled = false;
        isActivated = false;
        col.enabled = false;
        isBeingResurrected = false;

        NormalState();
        PlayerManager.Instance.PutPlayerToActiveState(player);
    }

    private void DownState()
    {
        player.GetComponentInChildren<PlayerSprite>().enabled = false;
        SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            if (sr == renderers[i])
            {
                sr.sprite = resSprite;
                continue;
            }

            renderers[i].color = stateColor;
        }
    }

    private void HealingState()
    {
        player.GetComponentInChildren<PlayerSprite>().enabled = false;
        SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            if(sr == renderers[i])
            {
                sr.sprite = healSprite;
                continue;
            }

            renderers[i].color = healStateColor;
        }
    }

    private void NormalState()
    {
        player.GetComponentInChildren<PlayerSprite>().enabled = false;
        SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            if(sr == renderers[i])
            {
                sr.sprite = null;
                continue;
            }
            renderers[i].color = originalColors[i];
        }
    }


}
