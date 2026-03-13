using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGunBehavior : MonoBehaviour
{
    [SerializeField] private Gun gun;

    private float fireUpdate = 0;
    private bool isBursting = false;
    private CircleCollider2D col;

    //enemies
    private int numEnemies = 0;
    [SerializeField] private List<GameObject> enemiesInRange = new List<GameObject>();

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        gun = transform.parent.GetComponent<PlayerInventory>().GetPlayerGunWeapon(); //(??)
        col.radius = gun.RangeRadius;

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


        int numBullets = (int)gun.GetPropertyByType(InfoType.BULLETS);
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

            yield return new WaitForSeconds(burstInterval); //small burst gap
        }

        yield return null;
    }
}
