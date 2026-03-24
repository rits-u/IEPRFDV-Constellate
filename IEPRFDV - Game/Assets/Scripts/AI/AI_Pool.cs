using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AI_Pool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public Transform parent;
    }


    [SerializeField] public List<Pool> pools;
    [SerializeField] public Dictionary<string, Queue<GameObject>> poolDictionary;
    [SerializeField] public Dictionary<string, Pool> poolLookup;

    #region Singleton
    public static AI_Pool instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolLookup = new Dictionary<string, Pool>();


        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
                
            poolLookup.Add(pool.tag, pool);
            for (int i = 0; i < pool.size; i++)
            {
                
                GameObject obj = null;
                //if (transform) obj = Instantiate(pool.prefab, transform, true);
                //else obj = Instantiate(pool.prefab);
                obj = Instantiate(pool.prefab);

                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    public GameObject SpawnFromPoolAsParent(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.transform.SetParent(GetPoolParent(tag), false);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    Transform GetPoolParent(string tag)
    {
        return poolLookup[tag].parent;
    }
}
