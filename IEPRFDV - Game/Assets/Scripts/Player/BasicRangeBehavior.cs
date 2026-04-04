using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRangeBehavior : MonoBehaviour
{
    [SerializeField] private Range gun;

    private float fireUpdate = 0;
    private bool isBursting = false;
    private CircleCollider2D col;

    //enemies
    private int numEnemies = 0;
    [SerializeField] private List<GameObject> enemiesInRange = new List<GameObject>();

    public bool isEnabled;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();

    }

    private void OnEnable()
    {
        isEnabled = true;
        gun = (Range)GetComponent<WeaponObject>().weapon;
    }

    private void OnDisable()
    {
        gun = null;
        col.radius = 0;
    }

    private void Update()
    {
        gun = (Range)GetComponent<WeaponObject>().weapon;

        if(isEnabled)
            col.radius = gun.RangeRadius;
        else 
            col.radius = 0;

        if (numEnemies <= 0)
            return;

        fireUpdate += Time.deltaTime;

        float fireRate = gun.GetPropertyByType(InfoType.FIRE_RATE);

        if (fireUpdate >= fireRate)
        {
            if (!isBursting)
            {
                StartCoroutine(FireBurstWrapper());
                fireUpdate = 0f;
            }
        }

        //Debug.Log(enemiesInRange);

    }


    //needs update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            numEnemies++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            numEnemies--;
        }
    }

    private GameObject FindNearestEnemy()
    {
        if (numEnemies == 0) return null;

        float shortestDist = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject enemy in enemiesInRange)
        {

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDist)
            {
                shortestDist = distance;
                nearest = enemy;
            }
        }

     //   Debug.Log(nearest);
        return nearest;
    }

    //private void FireBullets()
    //{
    //    for (int i = 0; i < numBullets; i++)
    //    {
    //        GameObject obj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    //        Vector3 direction = (FindNearestEnemy().transform.position - transform.position).normalized;
    //        obj.GetComponent<Bullet>().SetDirection(direction);
    //        bulletsSpawned.Add(obj);
    //    }
    //}

    //coroutine version
    IEnumerator FireBurstWrapper()
    {
        isBursting = true;
        yield return StartCoroutine(FireBullets());
        isBursting = false;
    }

    private IEnumerator FireBullets()
    {
        GameObject target = FindNearestEnemy();
        if (target == null) yield break;


        int numBullets = (int)gun.GetPropertyByType(InfoType.PROJECTILES);
        int damage = (int)gun.GetPropertyByType(InfoType.DAMAGE);
        float burstInterval = gun.GetPropertyByType(InfoType.BURST_INTERVAL);

        for (int i = 0; i < numBullets; i++)
        {
            if (target == null) yield break;

            GameObject obj = Instantiate(gun.BulletPrefab, transform.position, Quaternion.identity);

            Vector3 direction = (target.transform.position - transform.position).normalized;

            PlayerBullet bullet = obj.GetComponent<PlayerBullet>();
            Stats playerStats = GetComponentInParent<Stats>();
            bullet.SetDirection(direction);
            bullet.SetDamageInfo(playerStats.ATK + damage, playerStats.gameObject);

            AudioManager.Instance.PlaySFX(gun.SFX(), 0.3f);

            yield return new WaitForSeconds(burstInterval); //small burst gap
        }

        yield return null;
    }
}
