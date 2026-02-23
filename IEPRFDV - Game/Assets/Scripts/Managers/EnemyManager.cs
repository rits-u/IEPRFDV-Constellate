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

    private void CheckEnemyList()
    {
        //if(listEnemy.Count <= 0)
        //{
        //    EnemyManager.Instance.EndCurrentRound();
        //}
    }




}
