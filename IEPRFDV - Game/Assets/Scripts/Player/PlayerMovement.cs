using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask blockingLayer;

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 direction;
    private ContactFilter2D contactFilter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        move.action.Enable();

        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(blockingLayer);
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = false;
    }

    void Update()
    {
        direction = move.action.ReadValue<Vector2>();
        //Debug.Log(direction);
    }

    void FixedUpdate()
    {
        if (direction == Vector2.zero) return;

        //POSITION
        Vector2 movement = direction.normalized * moveSpeed * Time.fixedDeltaTime;
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int hitCount = col.Cast(movement, contactFilter, hits, movement.magnitude);

        if (hitCount == 0)
        {
            rb.MovePosition(rb.position + movement);
        }

        //ROTATION
        if(direction.x < 0)
        {
            Quaternion rotation = Quaternion.Euler(0f, 180f, 0f);
            transform.rotation = rotation;
        }
        else if(direction.x > 0)
        {
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
            transform.rotation = rotation;
        }

    }
}
