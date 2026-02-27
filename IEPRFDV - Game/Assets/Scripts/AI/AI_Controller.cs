using System.Collections;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{

    [SerializeField] private float projectileInterval = 0.5f;

    private AI_Pool projectilePool;
    private float nextProjectileTime = 0f;

    private Stats stats;


    void Start()
    {
        projectilePool = AI_Pool.instance;
        stats = GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
    }

    void Fire()
    {
        if (Time.time >= nextProjectileTime)
        {
            GameObject projectile = projectilePool.SpawnFromPool("Bullet", transform.position, transform.rotation);

            //initialize bullet dmg
            projectile.GetComponent<DamageDealer>().Damage = stats.ATK;

            nextProjectileTime = Time.time + projectileInterval;
        }
    }
}
