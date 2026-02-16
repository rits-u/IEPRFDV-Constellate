using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float interval;
   // public Vector3 direction;
  //  public float speed;
    private float timeSinceLast = 0;

    private void Update()
    {
        timeSinceLast += Time.deltaTime;

        if (timeSinceLast >= interval)
        {
          //  Debug.Log("hello world");
            SpawnBullet();
            timeSinceLast = 0;
        }

        
    }

    public void SpawnBullet()
    {
        //  GameObject obj = Instantiate(bulletPrefab, gameObject.transform);
        // obj.transform.position += direction * speed * Time.deltaTime;

        //    Vector3 spawnPos = transform.position + direction.normalized * 0.5f;
        GameObject obj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector3 direction = transform.right;
        obj.GetComponent<PlayerBullet>().SetDirection(direction);


    }
}
