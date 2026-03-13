using UnityEngine;
using System.Collections;

public class PlayerSlash : MonoBehaviour
{
    [SerializeField] private DamageInfo damageInfo;
    [SerializeField] private float lifeTime = 0.4f;    //duration
    [SerializeField] private float arcAngle = 140;     //swing Angle
    [SerializeField] private Vector3 startOffset = new Vector3(0.5f, 0, 0);
    [SerializeField] private float endOffsetAngle = 10f;
    [SerializeField] private bool fadeOut = true;

    private float startAngle, endAngle;
    private Transform pivot;
    private SpriteRenderer spriteRenderer;

    public struct DamageInfo
    {
        public int damage;
        public GameObject owner;
    }

    //void Awake()
    //{
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //}

    //void OnEnable()
    //{
    //    timer = 0f;
    //}

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
       /// timer = 0f; // only reset once
    }

    void OnEnable()
    {
        // Remove timer reset here
    }

    //void Update()
    //{
    //    if (pivot == null) return;

    //    timer += Time.deltaTime;
    //    float t = timer / lifeTime;

    // 
    //    float angle = Mathf.Lerp(startAngle, endAngle, t);
    //    transform.rotation = Quaternion.Euler(0, 0, angle);

    //    //mirror
    //    Vector3 facingOffset = startOffset;
    //    if (pivot.localScale.x < 0) // facing left
    //    {
    //        facingOffset.x *= -1; // mirror X
    //    }

    //    //pivot round
    //    transform.position = pivot.position + Quaternion.Euler(0, 0, angle) * facingOffset;


    //    //fade
    //    if (fadeOut && spriteRenderer != null)
    //    {
    //        Color c = spriteRenderer.color;
    //        c.a = Mathf.Lerp(1f, 0f, t);
    //        spriteRenderer.color = c;
    //    }

    //    if (timer >= lifeTime)
    //        Destroy(gameObject);
    //}

    public void PlaySlash()
    {
        StartCoroutine(SlashLifetime());
    }

    private IEnumerator SlashLifetime()
    {
        float t = 0f;
        while (t < lifeTime)
        {
            t += Time.deltaTime;

            //update rotate position
            float angle = Mathf.Lerp(startAngle, endAngle, t / lifeTime);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            Vector3 facingOffset = startOffset;
            if (pivot.localScale.x < 0) facingOffset.x *= -1;
            transform.position = pivot.position + Quaternion.Euler(0, 0, angle) * facingOffset;

            //fade
            if (fadeOut && spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = Mathf.Lerp(1f, 0f, t / lifeTime);
                spriteRenderer.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }




    public void SetDamageInfo(int dmg, GameObject owner)
    {
        damageInfo.damage = dmg;
        damageInfo.owner = owner;

        var dealer = GetComponent<DamageDealer>();
        if (dealer != null)
            dealer.Damage = damageInfo.damage;
    }

    //set pivot and direction
    public void SetDirection(Vector3 dir, Transform center)
    {
        pivot = center;

        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float halfArc = arcAngle * 0.5f;

        //mirror
        float startOffsetAngle;
        if (dir.x >= 0) //right
        {
            startOffsetAngle = 25f; 
        }
        else //left
        {
            startOffsetAngle = -25f; //mirror for left
        }

        //startAngle = baseAngle + startOffsetAngle + halfArc;
        //endAngle = baseAngle + startOffsetAngle - halfArc;


        startAngle = baseAngle + startOffsetAngle + halfArc;
        endAngle = baseAngle + startOffsetAngle - halfArc + endOffsetAngle;

        //initialize
        transform.position = pivot.position + Quaternion.Euler(0, 0, startAngle) * startOffset;
        transform.rotation = Quaternion.Euler(0, 0, startAngle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == damageInfo.owner) return;

        EnemyStats enemy = other.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.TakeDamage(damageInfo.damage, damageInfo.owner);
        }
    }
}