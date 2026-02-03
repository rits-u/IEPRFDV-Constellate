using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask blockingLayer;

    [SerializeField] private float doubleTapTime = 0.25f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.5f;

    private float lastTapTime;
    private int lastTapDir;   // -1 = left, 1 = right
    [SerializeField] private bool isDashing;

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

        //if (direction.x != 0)
        //{
        //    int tapDir = direction.x > 0 ? 1 : -1;

        //    if (Time.time - lastTapTime <= doubleTapTime &&
        //        tapDir == lastTapDir)
        //    {
        //        StartCoroutine(Dash(tapDir));
        //        lastTapTime = 0;
        //    }
        //    else
        //    {
        //        lastTapTime = Time.time;
        //        lastTapDir = tapDir;
        //    }
        //}


        if (move.action.WasPressedThisFrame())
        {
            if (Mathf.Abs(direction.x) > 0.5f)
            {
                int tapDir = direction.x > 0 ? 1 : -1;

                if (Time.time - lastTapTime <= doubleTapTime &&
                    tapDir == lastTapDir)
                {
                    StartCoroutine(Dash(tapDir));
                    lastTapTime = 0f;
                }
                else
                {
                    lastTapTime = Time.time;
                    lastTapDir = tapDir;
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

    IEnumerator Dash(int dir)
    {
        if (isDashing) yield break;

        isDashing = true;

        float timer = 0f;

        while (timer < dashDuration)
        {
            rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }
}
