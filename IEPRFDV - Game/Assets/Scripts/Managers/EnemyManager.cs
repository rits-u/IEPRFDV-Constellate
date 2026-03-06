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
    }

    private void UnRegisterEnemy(Stats enemy)
    {
        enemy.OnDeath -= UnRegisterEnemy;
        aliveEnemies -= 1;

        int index = 0;
        foreach (var e in listEnemy)
        {
            if(e == enemy.gameObject)
            {
                break;
            }
            index++;
        }

        listEnemy.RemoveAt(index);
        SpawnManager.Instance.UpdateCurrentSpawns(aliveEnemies);
        //CheckEnemyList();
        //Debug.Log("deleted " + index);
    }

    public void UnregisterAllEnemies()
    {
        foreach (var enemyObj in listEnemy)
        {
            Stats stats = enemyObj.GetComponent<Stats>();

            if (stats != null)
                stats.OnDeath -= UnRegisterEnemy;
        }

        listEnemy.Clear();
        aliveEnemies = 0;

        SpawnManager.Instance.UpdateCurrentSpawns(aliveEnemies);
    }

    public void DestroyAllEnemies()
    {
        Debug.Log($"EM: Start Destroy all enemies");
        for (int i = listEnemy.Count - 1; i >= 0; i--)
        {
            GameObject enemy = listEnemy[i];

            if (enemy == null) continue;

            Stats stats = enemy.GetComponent<Stats>();
            if (stats != null)
                stats.OnDeath -= UnRegisterEnemy;

           // Destroy(enemy);
            enemy.SetActive(false);
        }

        listEnemy.Clear();
        aliveEnemies = 0;

        SpawnManager.Instance.UpdateCurrentSpawns(aliveEnemies);
    }

    private void CheckEnemyList()
    {
        //if(listEnemy.Count <= 0)
        //{
        //    EnemyManager.Instance.EndCurrentRound();
        //}
    }




}
