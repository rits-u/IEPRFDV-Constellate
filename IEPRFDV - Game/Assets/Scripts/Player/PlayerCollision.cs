using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        if(damageDealer != null )
        {
            // playerStats.HP -= damageDealer.damage;
            playerStats.TakeDamage(damageDealer.damage);
           // Debug.Log($"Player HP: {playerStats.HP}");
        }
    }
}
