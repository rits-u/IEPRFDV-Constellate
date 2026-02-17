using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference move;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dash Properties")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.12f;
    [SerializeField] private float holdDashTime = 0.15f;   //how long you hold 2nd tap
    [SerializeField] private float tapWindow = 0.3f;       //max time between taps
    [SerializeField] private float dashCooldown = 2f;

    [SerializeField] private LayerMask blockingLayer;

    private Rigidbody2D rb;
    private Collider2D col;
    private ContactFilter2D contactFilter;

    //overall direction
    private Vector2 direction;

    //dash variables
    private bool isDashing;
    private float nextDashTime;

    //tap-hold detection
    private float firstTapTime;
    private float secondPressTime;
    private Vector2 firstTapDir;
    private bool secondPressDetected;

    private enum DashState { Idle, WaitingSecondTap, Dashing }
    private DashState dashState = DashState.Idle;

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
        Vector2 input = move.action.ReadValue<Vector2>();

        //movement input
        if (!isDashing)
            direction = input;

        //determine player's direction by their input
        Vector2 dominantDir = Vector2.zero;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            dominantDir = new Vector2(Mathf.Sign(input.x), 0);
        }
        else if (Mathf.Abs(input.y) > 0)
        {
            dominantDir = new Vector2(0, Mathf.Sign(input.y));
        }

        switch (dashState)
        {
            case DashState.Idle:
                //waits for first tap
                if (move.action.WasPressedThisFrame() && dominantDir != Vector2.zero)
                {
                    firstTapDir = dominantDir;
                    firstTapTime = Time.time;
                    dashState = DashState.WaitingSecondTap;
                }
                break;

            case DashState.WaitingSecondTap:
                //timeout if tap window is exceeded
                if (Time.time - firstTapTime > tapWindow)
                {
                    dashState = DashState.Idle;
                    secondPressDetected = false;
                    break;
                }

                //detects second tap in the same direction
                if (move.action.WasPressedThisFrame() && dominantDir == firstTapDir)
                {
                    secondPressTime = Time.time;
                    secondPressDetected = true;
                }

                //detect hold to trigger dash
                if (secondPressDetected && move.action.IsPressed() &&
                    !isDashing && Time.time >= nextDashTime)
                {
                    if (Time.time - secondPressTime >= holdDashTime)
                    {
                        dashState = DashState.Dashing;
                        secondPressDetected = false;
                        StartCoroutine(Dash(firstTapDir));
                    }
                }
                break;

            case DashState.Dashing: 
                break;
        }


        //if (Time.time >= nextDashTime)
        //    Debug.Log("dash ready");
        //else
        //    Debug.Log("cooldown");
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (direction == Vector2.zero) return;

        Vector2 movement = direction.normalized * moveSpeed * Time.fixedDeltaTime;

        // collision check
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int hitCount = col.Cast(movement, contactFilter, hits, movement.magnitude);
        if (hitCount == 0)
        {
            rb.MovePosition(rb.position + movement);
        }

        // rotation
        if (direction.x < 0)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (direction.x > 0)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    //IEnumerator Dash(Vector2 dir)
    //{
    //    if (isDashing) yield break;

    //    isDashing = true;
    //    nextDashTime = Time.time + dashCooldown;

    //    float timer = 0f;

    //    while (timer < dashDuration)
    //    {
    //        //stop dash if hitting wall
    //        if (IsHittingWall(dir))
    //            break;

    //        rb.linearVelocity = dir * dashSpeed;

    //        timer += Time.fixedDeltaTime;
    //        yield return new WaitForFixedUpdate();
    //    }

    //    rb.linearVelocity = Vector2.zero;
    //    isDashing = false;
    //    //   dashTriggered = false;
    //}

    IEnumerator Dash(Vector2 dir)
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;

        //Debug.Log(nextDashTime);

        float timer = 0f;
        while (timer < dashDuration)
        {
            if (IsHittingWall(dir)) break;
            rb.linearVelocity = dir * dashSpeed;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        dashState = DashState.Idle;
    }

    private bool IsHittingWall(Vector2 dir)
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];
        float checkDistance = dashSpeed * Time.fixedDeltaTime;

        int hitCount = col.Cast(dir, contactFilter, hits, checkDistance);
        return hitCount > 0;
    }
}
