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

    [Header("Spawn Intervals")]
    [SerializeField] private float minTime = 0.0f;
    [SerializeField] private float maxTime = 1.0f;
    [SerializeField] private int MaxSpawns = 15;

    [Header("Flags")]
    private int currentSpawns = 0;
    private Coroutine spawnRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("Spawn Manager: awake, Instancing self");
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
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    IEnumerator WaitTimer()
    {
        while (currentSpawns < MaxSpawns)
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
            if (currentSpawns < MaxSpawns)  //threshold
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
        Vector2 pos = (Vector2)transform.position;

        Vector2 newPos;
        switch (side)
        {
            case 0: //Up
                newPos = GetRandomSpawnPoint(outerX, -outerX, outerY, innerY);
                Debug.Log("SM: 0: " + newPos);
                return newPos;
            case 1: //Down
                newPos = GetRandomSpawnPoint(outerX, -outerX, -outerY, -innerY);
                Debug.Log("SM: 1: " + newPos);
                return newPos;
            case 2: //Left
                newPos = GetRandomSpawnPoint(-outerX, -innerX, outerY, -outerY);
                Debug.Log("SM: 2: " + newPos);
                return newPos;
            case 3: //Right
                newPos = GetRandomSpawnPoint(outerX, innerX, outerY, -outerY);
                Debug.Log("SM: 3: " + newPos);
                return newPos;
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
                Debug.Log("Spawn Manager: returning hit pos: " + hit.position);
                return hit.position;
            }
        }
        Vector2 debugVec = new Vector2(pointA1, pointB1);
        Debug.Log("Spawn Manager: fallback spawn pos = " +  debugVec);
        return debugVec;
    }


    void SpawnEnemy()
    {
        Debug.Log("SM: in spawnenemy");
        Vector2 randomSpawn = GetSpawnPoint();
        Vector3 spawnPos = new Vector3(randomSpawn.x, 0, randomSpawn.y);
        int enemyIndex = UnityEngine.Random.Range(0, enemyPrefabs.Length);

        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], randomSpawn, enemyPrefabs[enemyIndex].transform.rotation);
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
        else
        {
            Debug.Log("SM: navagent exists");
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

}
