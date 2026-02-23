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
            // playerStats.HP -= damageDealer.damage;
            // Debug.Log(other.name);
            if (this.CompareTag("Enemy"))
            {
                Debug.Log($"collided with [{other.name}]");
            }


            stats.TakeDamage(damageDealer.damage);
           // Debug.Log($"Player HP: {playerStats.HP}");
        }
    }
}
