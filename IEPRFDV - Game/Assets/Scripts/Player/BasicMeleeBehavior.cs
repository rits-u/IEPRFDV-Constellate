using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BasicMeleeBehavior : MonoBehaviour
{
    [SerializeField] private Melee melee;
    [SerializeField] private List<GameObject> enemiesInRange = new List<GameObject>();

    private float slashUpdate;
    private CircleCollider2D col;

    private int numEnemies = 0;
    public bool isEnabled;


    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        isEnabled = true;
        melee = (Melee)GetComponent<WeaponObject>().weapon;
        
    }

    private void OnDisable()
    {
        melee = null;
        isEnabled = false;
      //  col.radius = 0;
    }

    private void Update()
    {
        if (UIManager.Instance.isPaused) return;

        //   melee = (Melee)GetComponentInParent<PlayerInventory>().GetPlayerWeapon(); //(??)
        melee = (Melee)GetComponent<WeaponObject>().weapon;
        if(isEnabled)
            col.radius = melee.RangeRadius;
        else 
            col.radius = 0;

        if (numEnemies <= 0)
            return;

        slashUpdate += Time.deltaTime;

        float fireRate = melee.GetPropertyByType(InfoType.SLASH_INTERVAL);

        if (slashUpdate >= fireRate)
        {
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
        Stats playerStats = GetComponentInParent<Stats>();

        GameObject slashObj = Instantiate(melee.SlashPrefab, transform.position, Quaternion.identity);
        PlayerSlash slash = slashObj.GetComponent<PlayerSlash>();
        if (slash != null)
        {
            slash.SetDamageInfo(damage + playerStats.ATK, playerStats.gameObject);

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

            //sfx
            AudioManager.Instance.PlaySFX(melee.SFX(), 0.25f);
        }
    }


}
