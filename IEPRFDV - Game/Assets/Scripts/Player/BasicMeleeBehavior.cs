using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BasicMeleeBehavior : MonoBehaviour
{
    [SerializeField] private Melee melee;

    private float slashUpdate;
    private CircleCollider2D col;

    private int numEnemies = 0;
    [SerializeField] private List<GameObject> enemiesInRange = new List<GameObject>();

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        melee = (Melee)GetComponent<WeaponObject>().weapon;
        
    }

    private void OnDisable()
    {
        melee = null;
      //  col.radius = 0;
    }

    private void Update()
    {
        //   melee = (Melee)GetComponentInParent<PlayerInventory>().GetPlayerWeapon(); //(??)
        melee = (Melee)GetComponent<WeaponObject>().weapon;
        col.radius = melee.RangeRadius;

        if (numEnemies <= 0)
            return;

        slashUpdate += Time.deltaTime;

        float fireRate = melee.GetPropertyByType(InfoType.SLASH_INTERVAL);

        if (slashUpdate >= fireRate)
        {
            //if (!isBursting)
            //{
            //    StartCoroutine(FireBurstWrapper());
            //    fireUpdate = 0f;
            //}
            Slash();
            slashUpdate = 0f;
        }


        //Debug.Log(enemiesInRange);

    }



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
        return nearest;
    }



    private void Slash()
    {
        int damage = (int)melee.GetPropertyByType(InfoType.DAMAGE);

        GameObject slashObj = Instantiate(melee.SlashPrefab, transform.position, Quaternion.identity);

        PlayerSlash slash = slashObj.GetComponent<PlayerSlash>();
        if (slash != null)
        {
            slash.SetDamageInfo(damage, gameObject);

            GameObject target = FindNearestEnemy();
            Vector3 direction;

            if (target != null)
            {
                direction = (target.transform.position - transform.position).normalized;
            }
            else
            {
                direction = transform.localScale.x >= 0 ? Vector3.right : Vector3.left;
            }

            //set pivot
            slash.SetDirection(direction, transform);

            slash.PlaySlash();
        }
    }


}
