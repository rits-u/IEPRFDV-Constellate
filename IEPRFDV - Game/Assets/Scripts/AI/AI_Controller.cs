using System;
using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AI_Controller : MonoBehaviour
{

    [Header("References")]
    private AI_Pool projectilePool;
    [SerializeField] private GameObject meleeHitbox;
    [SerializeField] private DamageDealer damageDealer;
    private NavMeshAgent navAgent;

    [Header("Melee")]
    [SerializeField] private bool hasMelee = false;
    [SerializeField] private bool meleeAttackOnProximity = false;
    [SerializeField] private int meleeDamage = 2;
    [SerializeField] private float meleeInterval = 1f;
    [SerializeField] private float meleeDuration = 1f;
    [SerializeField] private float stoppingRange = 0.4f;

    [Header("Ranged")]
    [SerializeField] private bool hasRanged = false;
    [SerializeField] private int rangedDamage = 1;
    [SerializeField] private float rangedInterval = 2f;

    private float meleeTimer = 0f;
    private float rangedTimer = 0f;

    private void Awake()
    {
        InitializeReferences();
        InitializeValues();
    }

    void Start()
    {
        projectilePool = AI_Pool.instance;
    }

    // Update is called once per frame
    void Update()
    {

        if (hasMelee)
        {
            meleeTimer -= Time.deltaTime;
            Melee();
        }
        else if (hasRanged)
        {
            rangedTimer -= Time.deltaTime;
            Ranged();
        }
        
    }

    void Melee()
    {
        //use meleehitbox to check if any collision
        if (meleeTimer <= 0f)
        {
            StartCoroutine(MeleeAttack());
            meleeTimer = meleeInterval;
        }
    }
    void Ranged()
    {
        if (rangedTimer <= 0f)
        {
            GameObject projectile = projectilePool.SpawnFromPool("Bullet", transform.position, transform.rotation);
            rangedTimer = rangedInterval;
        }
    }

    private IEnumerator MeleeAttack()
    {
        meleeHitbox.SetActive(true);
        yield return new WaitForSeconds(meleeDuration);
        meleeHitbox.SetActive(false);
    }

    private void InitializeValues()
    {
        
        meleeTimer = meleeInterval;
        rangedTimer = 0.4f;
        if (hasMelee && hasRanged)
        {
            Debug.Log($"{gameObject.name} cannot have two damage types");
        }
        if (hasMelee)
        {
            damageDealer.damage = meleeDamage;
            navAgent.stoppingDistance = stoppingRange;
        }
        if (hasRanged) damageDealer.damage = rangedDamage;
    }

    private void InitializeReferences()
    {
        navAgent = GetComponent<NavMeshAgent>();

        if (!hasMelee)
        {
            Transform child = transform.Find("Melee Hitbox");
            if (child != null)
            {
                meleeHitbox = child.gameObject;
            }
            else
            {
                Debug.LogError($"{gameObject} has no gameobject Melee Hitbox");
            }
        }


        if (!damageDealer)
        {
            damageDealer = GetComponent<DamageDealer>();
        }

    }

}
