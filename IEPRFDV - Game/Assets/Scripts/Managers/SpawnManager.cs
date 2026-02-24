using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Constants")]
    [HideInInspector] private int maxRetries = 15;

    [Header("References")]
    [SerializeField] private NavMeshAgent spawnArea;
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
        if (Instance == null) Instance = this;
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
            float waitTime = Random.Range(minTime, maxTime);
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

            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector2 GetSpawnPoint()
    {
        int side = Random.Range(0, 4);
        Vector2 pos = (Vector2)transform.position;

        switch (side)
        {
            case 0: //Up
                return GetRandomSpawnPoint(outerX, -outerX, outerY, innerY) + pos;
            case 1: //Down
                return GetRandomSpawnPoint(outerX, -outerX, -outerY, -innerY) + pos;
            case 2: //Left
                return GetRandomSpawnPoint(-outerX, -innerX, outerY, -outerY) + pos;
            case 3: //Right
                return GetRandomSpawnPoint(outerX, innerX, outerY, -outerY) + pos;
        }
        return Vector2.zero;
    }

    Vector2 GetRandomSpawnPoint(float pointA1, float pointA2, float pointB1, float pointB2)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            Vector2 randomPoint = new Vector2(Random.Range(pointA1, pointA2), Random.Range(pointB1, pointB2));
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas)){
                return hit.position;
            }
        }
        return new Vector2(pointA1, pointB1);
    }

    void SpawnEnemy()
    {
        Vector2 randomSpawn = GetSpawnPoint();
        Vector3 spawnPos = new Vector3(randomSpawn.x, 0, randomSpawn.y);
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);

        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPos, enemyPrefabs[enemyIndex].transform.rotation);

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
