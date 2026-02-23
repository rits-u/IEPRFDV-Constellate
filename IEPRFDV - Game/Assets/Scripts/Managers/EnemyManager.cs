using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    [SerializeField] private List<GameObject> listEnemy = new();
    [SerializeField] private int aliveEnemies;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        listEnemy.Add(enemy);
        aliveEnemies += 1;
        Stats enemyStats = enemy.GetComponent<Stats>();
        enemyStats.OnDeath += UnRegisterEnemy;
        //enemy.GetComponent<Stats>(enemy.GetComponent<Stats>()).OnDeath += HandleEnemyDeath;
    }

    private void UnRegisterEnemy(Stats enemy)
    {
        aliveEnemies -= 1;
        foreach (var e in listEnemy)
        {
            if(e == enemy.gameObject)
            {
                listEnemy.Remove(e);
            }
        }

        enemy.OnDeath -= UnRegisterEnemy;
    }




}
