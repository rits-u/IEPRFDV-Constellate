using UnityEngine;

public class PlayerBullet : MonoBehaviour 
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private int damage;
    private Vector3 direction;

    void OnEnable()
    {
        Invoke(nameof(DestroySelf), lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
        GetComponent<DamageDealer>().Damage = damage;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
            Destroy(gameObject);

       // if()
       // Destroy(gameObject);

    }
}