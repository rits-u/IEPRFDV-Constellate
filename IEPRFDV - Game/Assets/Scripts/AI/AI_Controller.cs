using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AI_Controller : MonoBehaviour
{

    [Header("References")]
    private AI_Pool projectilePool;
    [SerializeField] private GameObject meleeHitbox;
    [SerializeField] private DamageDealer damageDealer;
    [SerializeField] private Animator animator;
    private NavMeshAgent navAgent;
    private AI_FollowPlayer followPlayer;

    [Header("Melee")]
    [SerializeField] private bool hasMelee = false;
    //[SerializeField][ShowIf("hasMelee")] private bool meleeAttackOnProximity = false;
    [SerializeField][ShowIf("hasMelee")] private int meleeDamage = 2;
    [SerializeField][ShowIf("hasMelee")] private float meleeInterval = 1f;
    [SerializeField][ShowIf("hasMelee")] private float meleeDuration = 1f;
    [SerializeField][ShowIf("hasMelee")] private float stoppingRange = 0.4f;

    [Header("Ranged")]
    [SerializeField] private bool hasRanged = false;
    [SerializeField][ShowIf("hasRanged")] private GameObject bullet;
    [SerializeField][ShowIf("hasRanged")] private int rangedDamage = 1;
    [SerializeField][ShowIf("hasRanged")] private float rangedInterval = 2f;

    private float meleeTimer = 0f;
    private float rangedTimer = 0f;
    private string bulletName;

    private void Awake()
    {
        InitializeReferences();
        InitializeValues();
    }

 //   private AI_Pool projectilePool;
    private float nextProjectileTime = 0f;

    private Stats stats;


    void Start()
    {
        projectilePool = AI_Pool.instance;
        stats = GetComponent<Stats>();
        if (hasRanged) projectilePool = AI_Pool.instance;


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
            GameObject projectile;
            if (!(projectile = projectilePool.SpawnFromPool(bulletName, transform.position, GetTargetRotation())))
            {
                Debug.Log($"{transform.name} projectile fail");
            }
            
            
            GameObject projectile = projectilePool.SpawnFromPool("Bullet", transform.position, GetTargetRotation());
            projectile.GetComponent<DamageDealer>().Damage = stats.ATK;
            rangedTimer = rangedInterval;
        }
    }

    private IEnumerator MeleeAttack()
    {
        meleeHitbox.SetActive(true);
        yield return new WaitForSeconds(meleeDuration);
        meleeHitbox.SetActive(false);
    }
    private Quaternion GetTargetRotation()
    {
        Vector3 direction = Vector3.zero;

        if (followPlayer)
        {
            GameObject target = followPlayer.GetTarget();
            if (target != null) direction = target.transform.position - transform.position;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90f;
        return Quaternion.Euler(0, 0, angle);
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
            damageDealer.Damage = meleeDamage;
            navAgent.stoppingDistance = stoppingRange;
        }
        if (hasRanged) damageDealer.Damage = rangedDamage;
    }

    private void InitializeReferences()
    {
        navAgent = GetComponent<NavMeshAgent>();
        
        followPlayer = GetComponent<AI_FollowPlayer>();

        //if (!animator) animator = transform.GetComponent<Animator>();
        if (!hasMelee)
        {
            Transform child = transform.Find("Melee Hitbox");
            if (child != null)  meleeHitbox = child.gameObject;
            else Debug.LogError($"{gameObject} has no gameobject Melee Hitbox");
        }

        if (!damageDealer) damageDealer = GetComponent<DamageDealer>();

        if (hasRanged)
        {
            if (!bullet) Debug.LogError($"{transform.name} no bullet reference");
            bulletName = bullet.name;
            if (bulletName == null) Debug.LogError($"{transform.name} bullet name is empty");
        }
        
    }


