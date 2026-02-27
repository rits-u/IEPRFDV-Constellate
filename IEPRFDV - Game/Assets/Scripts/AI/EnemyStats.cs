using UnityEngine;

public class EnemyStats : Stats
{
    [Header("Enemy Point Value")]
    [Tooltip("Amount of points that player will get when defeated")]
    [SerializeField] private int pointValue;

    public int PointValue
    {
        get => pointValue;
        set => pointValue = value;
    }

    public void TakeDamage(int damage, GameObject source)
    {
        bool enemyDied = TakeDamage(damage);

        if (enemyDied)
        {
            if (PlayerManager.Instance != null)
            {
                PlayerManager.Instance.AddPointToPlayer(source, pointValue);
            }
            else
            {
                Debug.Log("PlayerManager is not present in the scene.");
            }
          
        }


    }
}
