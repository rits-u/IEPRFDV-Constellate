using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Reference")]
    [SerializeField] private InputActionReference move;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float doubleTapTime = 0.25f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private bool isDashing;

    [SerializeField] private LayerMask blockingLayer;

    //for dash
    private Vector2 lastTapDir;
    private float lastTapTime;

    //rigidbody
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

        if(move.action.WasPressedThisFrame())
        {     
            //determine dominant axis
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction = new Vector2(Mathf.Sign(direction.x), 0);
            }
            else
            {
                direction = new Vector2(0, Mathf.Sign(direction.y));
            }

            if (direction != Vector2.zero)
            {
                if (Time.time - lastTapTime <= doubleTapTime &&
                    direction == lastTapDir)
                {
                    StartCoroutine(Dash(direction));
                    lastTapTime = 0f;
                }
                else
                {
                    lastTapTime = Time.time;
                    lastTapDir = direction;
                }
            }
        }
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
        if (direction.x < 0)
        {
            Quaternion rotation = Quaternion.Euler(0f, 180f, 0f);
            transform.rotation = rotation;
        }
        else if (direction.x > 0)
        {
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
            transform.rotation = rotation;
        }

    }

    IEnumerator Dash(Vector2 dir)
    {
        if (isDashing) yield break;

        isDashing = true;

        float timer = 0f;

        while (timer < dashDuration)
        {
            rb.linearVelocity = dir * dashSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }
}
