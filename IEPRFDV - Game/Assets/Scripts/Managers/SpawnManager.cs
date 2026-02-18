using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float minTime = 0.0f;
    [SerializeField] private float maxTime = 1.0f;
    [SerializeField] private int MaxSpawns = 15;
    
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] public Vector2 spawnPerimeter;

    private int currentSpawns = 0;
    void Start()
    {
        StartCoroutine(WaitTimer());
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

        Instantiate(enemyPrefabs[enemyIndex], spawnPos, enemyPrefabs[enemyIndex].transform.rotation);
    }

}
