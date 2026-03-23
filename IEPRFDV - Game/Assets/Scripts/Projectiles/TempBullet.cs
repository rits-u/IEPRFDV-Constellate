using UnityEngine;

public class TempBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 4f;
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private bool hasRotate = false;

    private GameObject bullet;
    //private GameObject trail;
    void Start()
    {
        transform.Rotate(rotationOffset);
        bullet = transform.Find("Bullet").GetComponent<GameObject>();
        //trail = transform.Find("Trail").GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (hasRotate) Rotate();
    }

    void Move()
    {
        Vector3 direction = transform.rotation * Vector3.up;
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }
    void Rotate()
    {
        bullet.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}