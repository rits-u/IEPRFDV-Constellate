using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] private float minTime = 0.0f;
    [SerializeField] private float maxTime = 1.0f;
    [SerializeField] private int MaxSpawns = 15;
    
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] public Vector2 spawnPerimeter;

    private int currentSpawns = 0;
    private Coroutine spawnRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
        float halfX = spawnPerimeter.x / 2f;
        float halfY = spawnPerimeter.y / 2f;
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0:
                return new Vector2(Random.Range(-halfX, halfX), halfY) + (Vector2)transform.position;
            case 1:
                return new Vector2(Random.Range(-halfX, halfX), -halfY) + (Vector2)transform.position;
            case 2:
                return new Vector2(-halfX, Random.Range(-halfY, halfY)) + (Vector2)transform.position;
            case 3:
                return new Vector2(halfX, Random.Range(-halfY, halfY)) + (Vector2)transform.position;
        }
        return Vector2.zero;
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
