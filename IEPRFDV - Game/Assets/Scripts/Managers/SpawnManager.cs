using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Threading;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Constants")]
    [HideInInspector] private int maxRetries = 15;

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject outerSpawnPerimiterTopRight;
    [SerializeField] private GameObject innerSpawnPerimiterTopRight;
    [HideInInspector] private float outerX;
    [HideInInspector] private float outerY;
    [HideInInspector] private float innerX;
    [HideInInspector] private float innerY;

    [Header("Enemy Spawn Initialization")]
    [Header("Quantity")]
    [SerializeField] private int enemyCount;
    [SerializeField] private int updateCountEvery;
    [SerializeField] private int enemyIncrement;

    [Header("Spawn Intervals")]
    [SerializeField] private float minTime = 0.0f;
    [SerializeField] private float maxTime = 1.0f;
    [SerializeField] private int maxSpawns = 15;

    [Header("Flags")]
    private int currentSpawns = 0;
    private Coroutine spawnRoutine;

    private bool isFirstRound = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        if (!outerSpawnPerimiterTopRight) Debug.LogError("SpawnManager: outer spawn perimeter top right is null");
        else
        {
            Vector2 outerSpawnPerimiter = outerSpawnPerimiterTopRight.transform.position;
            outerX = outerSpawnPerimiter.x;
            outerY = outerSpawnPerimiter.y;
        }
        if (!innerSpawnPerimiterTopRight) Debug.LogError("SpawnManager: inner spawn perimeter top right is null");
        else
        {
            Vector2 innerSpawnPerimiter = innerSpawnPerimiterTopRight.transform.position;
            innerX = innerSpawnPerimiter.x;
            innerY = innerSpawnPerimiter.y;
        }

    }

    void Start()
    {
        // StartCoroutine(WaitTimer());
    }

    public void StartSpawning()
    {
        currentSpawns = 0;
        InitializeSpawn();
        MaxSpawns = enemyCount;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    IEnumerator WaitTimer()
    {
        while (currentSpawns < maxSpawns)
        {
            float waitTime = UnityEngine.Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            SpawnEnemy();
            currentSpawns++;
        }
        yield return null;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentSpawns < maxSpawns)  //threshold
            {
                SpawnEnemy();
                currentSpawns++;
            }

            float waitTime = UnityEngine.Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector2 GetSpawnPoint()
    {
        int side = UnityEngine.Random.Range(0, 4);

        switch (side)
        {
            case 0: //Up
                return GetRandomSpawnPoint(outerX, -outerX, outerY, innerY);
            case 1: //Down
                return GetRandomSpawnPoint(outerX, -outerX, -outerY, -innerY);
            case 2: //Left
                return GetRandomSpawnPoint(-outerX, -innerX, outerY, -outerY);
            case 3: //Right
                return GetRandomSpawnPoint(outerX, innerX, outerY, -outerY);
        }
        return Vector2.zero;
    }

    Vector2 GetRandomSpawnPoint(float pointA1, float pointA2, float pointB1, float pointB2)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            Vector2 randomPoint = new Vector2(UnityEngine.Random.Range(pointA1, pointA2), UnityEngine.Random.Range(pointB1, pointB2));
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        Vector2 debugVec = new Vector2(pointA1, pointB1);
        return debugVec;
    }


    void SpawnEnemy()
    {
        Vector2 spawnPos = GetSpawnPoint();
        int enemyIndex = UnityEngine.Random.Range(0, enemyPrefabs.Length);

        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPos, enemyPrefabs[enemyIndex].transform.rotation);
        NavMeshAgent navAgent = enemy.GetComponent<NavMeshAgent>();
        if (!navAgent)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas))
            {
                Debug.LogWarning("SM: Enemy spawn pos is on navmesh " + hit.position);
                navAgent.Warp(hit.position);
            }
            else
            {
                Debug.LogWarning("SM: Enemy spawn pos not on navmesh");
            }
        }
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterEnemy(enemy);
        }
    }

    public void UpdateCurrentSpawns(int numEnemies)
    {
        currentSpawns = numEnemies;
        //Debug.Log($"Current Spawned: {currentSpawns}");
    }

    private void InitializeSpawn()
    {
        if (isFirstRound)
        {
            isFirstRound = false;
            return;
        }

        int round = RoundManager.Instance.RoundNumber;
        if (round % updateCountEvery == 0)
        {
            enemyCount += enemyIncrement;
        }

    }

    public void UpdateSpawnInterval(int minTime, int maxTime, int maxSpawns)
    {
        if (minTime != 0) this.minTime = minTime;
        if (maxTime != 0) this.maxTime = maxTime;
        if (maxSpawns != 0) this.maxSpawns = maxSpawns;
    }

}
