using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    [SerializeField] private List<GameObject> listEnemy = new();
    [SerializeField] private int aliveEnemies;

    [Header("Enemy Stats Initialization")]
    [Header("HP")]
    [SerializeField] private int enemyStartingHP;
    [SerializeField] private int updateHealthEvery;
    [SerializeField] private int healthIncrement;


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
        //initialize stats
        InitializeEnemy(enemy);

        listEnemy.Add(enemy);
        aliveEnemies += 1;
    //    Stats enemyStats = enemy.GetComponent<Stats>();
        //enemyStats.OnDeath += UnRegisterEnemy;
    }

    private void UnRegisterEnemy(Stats enemyStats)
    {
        enemyStats.OnDeath -= UnRegisterEnemy;
        aliveEnemies -= 1;

        int index = 0;
        foreach (var e in listEnemy)
        {
            if(e == enemyStats.gameObject)
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
            Stats enemyStats = enemyObj.GetComponent<Stats>();

            if (enemyStats != null)
                enemyStats.OnDeath -= UnRegisterEnemy;
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

            Stats enemyStats = enemy.GetComponent<Stats>();
            if (enemyStats != null)
                enemyStats.OnDeath -= UnRegisterEnemy;

           // Destroy(enemy);
            enemy.SetActive(false);
        }

        listEnemy.Clear();
        aliveEnemies = 0;

        SpawnManager.Instance.UpdateCurrentSpawns(aliveEnemies);
    }

    private void InitializeEnemy(GameObject enemy)
    {
        Stats enemyStats = enemy.GetComponent<Stats>();
        enemyStats.OnDeath += UnRegisterEnemy;

        enemyStartingHP = enemyStats.HP;

        int round = RoundManager.Instance.RoundNumber;

        if (round % updateHealthEvery == 0)
        {
            enemyStartingHP += healthIncrement;
        }

        enemyStats.HP = enemyStartingHP;

        //  int round = 
    }

    //public int GetMaxEnemySpawns()
    //{
    //    return enemyCount;
    //}

    private void CheckEnemyList()
    {
        //if(listEnemy.Count <= 0)
        //{
        //    EnemyManager.Instance.EndCurrentRound();
        //}
    }




}
