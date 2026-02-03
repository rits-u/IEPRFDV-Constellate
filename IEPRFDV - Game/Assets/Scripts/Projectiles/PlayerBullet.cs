using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    private Vector3 direction;

    void OnEnable()
    {
        Invoke(nameof(DestroySelf), lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}