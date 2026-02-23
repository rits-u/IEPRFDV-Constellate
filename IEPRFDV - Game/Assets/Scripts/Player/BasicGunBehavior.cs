using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGunBehavior : MonoBehaviour
{
    //[Header("Fields")]
    //[SerializeField] private int numBullets = 3;
    //[SerializeField] private float burstInterval = 0.15f;
    //[SerializeField] private float fireInterval = 2f;

    //[SerializeField] GameObject bulletPrefab;
    [SerializeField] GunTemplate gun;

    private float fireUpdate = 0;
    private bool isBursting = false;

    //enemies
    private int numEnemies = 0;
    [SerializeField] private List<GameObject> enemiesInRange = new List<GameObject>();

    private void Start()
    {
    }

    private void Update()
    {
        if (numEnemies <= 0)
            return;

        fireUpdate += Time.deltaTime;

        //if (fireUpdate >= fireInterval)
        //{
        //    //FireBullets();
        //    StartCoroutine(FireBullets());
        //    fireUpdate = 0f;
        //}

        if (fireUpdate >= gun.FireInterval)
        {
            if (!isBursting)
            {
                StartCoroutine(FireBurstWrapper());
                fireUpdate = 0f;
                Debug.Log("Fire");
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

        for (int i = 0; i < gun.NumBullets; i++)
        {
            if (target == null) yield break;

            GameObject obj = Instantiate(gun.BulletPrefab, transform.position, Quaternion.identity);

            Vector3 direction = (target.transform.position - transform.position).normalized;

            obj.GetComponent<PlayerBullet>().SetDirection(direction);

            yield return new WaitForSeconds(gun.BurstInterval); //small burst gap
        }
    }
}
