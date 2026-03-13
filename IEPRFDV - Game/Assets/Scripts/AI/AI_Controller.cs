using System;
using System.Collections;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{

    [SerializeField] private float projectileInterval = 0.5f;

    private AI_Pool projectilePool;
    private float nextProjectileTime = 0f;

    [Header("Melee")]
    [SerializeField] private bool hasMelee = false;
    [SerializeField] private float meleeInterval = 1f;
    [SerializeField] private float meleeRange = 2f;

    [Header("Ranged")]
    [SerializeField] private bool hasRanged = false;
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
        float distance = Vector3.Distance(transform.position, transform.forward);
        if (distance <= meleeRange && meleeTimer <= 0f)
        {
            Attack();
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

    void Attack()
    {
        Debug.Log("Enemy attack");
    }

    private void InitializeValues()
    {
        
        meleeTimer = meleeInterval;
        rangedTimer = 0.4f;
    }

    private void InitializeReferences()
    {

    }

}
