using UnityEngine;

public class TempBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 4f;
    [SerializeField] private Vector3 rotationOffset;
    void Start()
    {
        transform.Rotate(rotationOffset);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = transform.rotation * Vector3.up;
        transform.position += direction * bulletSpeed * Time.deltaTime;
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
}
