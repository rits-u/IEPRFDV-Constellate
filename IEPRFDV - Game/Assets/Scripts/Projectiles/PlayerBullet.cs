using UnityEngine;

public class PlayerBullet : MonoBehaviour 
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private DamageInfo damageInfo;
    private Vector3 direction;

    public struct DamageInfo
    {
        public int damage;
        public GameObject owner;
    }
    

    void OnEnable()
    {
        Invoke(nameof(DestroySelf), lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDamageInfo(int dmg, GameObject owner)
    {
        damageInfo.damage = dmg;
        damageInfo.owner = owner;
        GetComponent<DamageDealer>().Damage = damageInfo.damage;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        transform.right = direction;
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }


        EnemyStats enemy = other.GetComponent<EnemyStats>();
        if(enemy != null)
        {
           
            enemy.TakeDamage(damageInfo.damage, damageInfo.owner);
        }

       // if()
       // Destroy(gameObject);

    }
}