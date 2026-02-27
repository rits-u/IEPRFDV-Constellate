using UnityEngine;

public class EntityCollision : MonoBehaviour
{
    private Stats stats;

    private void Start()
    {
        stats = GetComponent<Stats>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        if(damageDealer != null )
        {
            stats.TakeDamage(damageDealer.Damage);
        }
    }
}
