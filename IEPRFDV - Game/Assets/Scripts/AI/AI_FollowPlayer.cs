using NaughtyAttributes;
using System.Collections;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class AI_FollowPlayer : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject[] players;
    private GameObject target = null;

    [Header("Consts")]
    //[SerializeField] private float moveSpeed = 0.8f;
    //[SerializeField] private bool randomMovement = false;
    //[SerializeField] private bool hasLimitedVisibility = false;
    //[ShowIf("hasLimitedVisibility")][SerializeField] private float detectionRadius = 5.0f;
    //[ShowIf("randomMovement")][SerializeField] private float randomMovementRange = 5.0f;
    [SerializeField] private const float pathRefreshTime = 3.0f;
    [SerializeField] private const float moveSpeed = 2.6f;
    [SerializeField] private const float acceleration = 8f;

    [Header("Dash")]
    [SerializeField] private bool hasDash = false;
    [ShowIf("hasDash")][SerializeField] private bool dashRandomInterval = true;
    [ShowIf("hasDash")][SerializeField] private float dashDuration = 1.2f;
    [HideIf("dashRandomInterval")][SerializeField] private float dashIntervalSeconds = 2f;
    [ShowIf("dashRandomInterval")][SerializeField] private float dashRandomMaxSeconds = 4f;
    [ShowIf("dashRandomInterval")][SerializeField] private float dashRandomMinSeconds = 2f;
    [ShowIf("hasDash")][SerializeField] private float dashSpeedMult = 1.5f;
    [ShowIf("hasDash")][SerializeField] private float dashAccelerationMult = 2f;
    [ShowIf("hasDash")][SerializeField] private bool dashStopMovement = false;

    [Header("Teleport")]
    [SerializeField] private bool hasTeleport = false;
    [ShowIf("hasTeleport")][SerializeField] private bool TPRandomInterval = false;
    [ShowIf("hasTeleport")][SerializeField] private float TPInterval = 4f;
    [ShowIf("hasTeleport")][SerializeField] private float TPDist = 0.5f;
    [ShowIf("hasDash")][SerializeField] private bool TPHasDash = false;
    [ShowIf("hasTeleport")][SerializeField] private bool TPStopMovement = false;

    [Header("Attack")]
    [SerializeField] private bool stopOnAttack = false;
    //[SerializeField] private float cooldown;

    [Header("Flags")]
    [HideInInspector] private bool canDash = true;
    [HideInInspector] private bool canTeleport = true;

    private Vector3 positionOffset;
    private NavMeshAgent navAgent;
    //private Animator animator;
    //private float detectionBuffer = 1.0f;
    private float targetDistance;

    private float dashTime = 0f;
    private Vector3 dashDirection;
    private Vector3 lastPosition;
    private bool hasDebug = true;
    private bool debugStopMovement = true;

    private void Awake()
    {
        InitializeReferences();
        InitializeValues();
    }
    void Start()
    {
        //if (attackDistance < detectionRadius)
        //{
        //    detectionRadius = attackDistance + 3.0f;
        //}

        if (players == null || players.Length == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        if (players != null && players.Length > 0)
        {
            target = players[0];
        }
        if (target)
        {
            target.GetComponent<Stats>().OnDeath += OnTargetDeath;
            SetRotation();
            StartCoroutine(WaitTimer());
        }

    }

    IEnumerator WaitTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(pathRefreshTime);
            //target = GetTarget();
            SetTarget(GetTarget());
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        SetRotation();

        if (canDash && hasDash)
        {
            canDash = false;
            StartCoroutine(Dash());
        }
        else if (hasDash && !dashStopMovement)
        {
            navAgent.SetDestination(target.transform.position);
        }

        if (canTeleport && hasTeleport)
        {
            canTeleport = false;
            StartCoroutine(Teleport());
        }
        else if (hasTeleport && !TPStopMovement)
        {
            //navAgent.SetDestination(target.transform.position);
        }

        


        //}   
        //}
        //else if (randomMovementRange && targetDistance > detectionRadius + detectionBuffer)
        //{
        //    //Prevents auto braking from stopping random movement
        //    if (navAgent.remainingDistance > navAgent.stoppingDistance)
        //    {

        //    }
        //    else
        //    {
        //        Vector2 randomOffset = Random.insideUnitCircle * randomMovementRange;
        //        Vector3 randomDest = navAgent.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
        //        navAgent.SetDestination(randomDest);
        //    }
        //}
    }

    GameObject GetTarget()
    {
        if (players == null || players.Length == 0)
        {
            return null;
        }

        float shortest = Mathf.Infinity;
        GameObject toFollow = null;
        foreach (GameObject player in players)
        {
            if (player == null) continue;

            Stats stats = player.GetComponent<Stats>();
            if (stats == null || stats.HP <= 0) continue;

            Vector2 pos = player.transform.position - transform.position;
            float distSqr = pos.sqrMagnitude;
            if (distSqr < shortest)
            {
                shortest = distSqr;
                toFollow = player;
            }
        }
        return toFollow;
    }

    private Stats currentTargetStats;

    void SetTarget(GameObject newTarget)
    {
        if (currentTargetStats != null)
            currentTargetStats.OnDeath -= OnTargetDeath;

        target = newTarget;

        if (target != null)
        {
            lastPosition = target.transform.position;
            currentTargetStats = target.GetComponent<Stats>();
            if (currentTargetStats != null)
            {
                currentTargetStats.OnDeath += OnTargetDeath;
            }
        }
    }

    private void SetRotation()
    {
        Vector3 direction = target.transform.position - navAgent.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90f;
        navAgent.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTargetDeath(Stats s)
    {
        target = null;
    }

    //private void OnAnimatorMove()
    //{
    //    if (animator.GetBool("Attack") == false)
    //    {
    //        navAgent.speed = (animator.deltaPosition / Time.deltaTime).magnitude;
    //    }
    //}

    private IEnumerator Dash()
    {
        float totalInterval;
        if (dashRandomInterval)
        {
            totalInterval = Random.Range(dashRandomMinSeconds, dashRandomMaxSeconds);
        }
        else
        {
            totalInterval = dashIntervalSeconds;  
        }

        navAgent.speed = Mathf.Clamp(dashSpeedMult * moveSpeed, 0f, 100000f);
        navAgent.acceleration = Mathf.Clamp(dashAccelerationMult * acceleration, 0f, 100000f);

        float time = 0f;
        while (time < dashDuration)
        {
            time += Time.deltaTime;
            if (target) navAgent.SetDestination(target.transform.position);
            else navAgent.SetDestination(lastPosition);

            yield return null;
        }

        navAgent.speed = moveSpeed;
        navAgent.acceleration = acceleration;

        if (dashStopMovement)
        {
            navAgent.ResetPath();
        }

        yield return new WaitForSeconds(totalInterval);
        canDash = true;
    }

    private IEnumerator Teleport()
    {
        Vector3 direction;
        if (target)
            direction = (target.transform.position - transform.position).normalized;
        else
            direction = transform.forward;

        float time = 0f;
        while (time < TPInterval)
        {
            time += Time.deltaTime;

            if (!TPStopMovement)
            {
                if (target) navAgent.SetDestination(target.transform.position);
                else navAgent.SetDestination(lastPosition);
            }

            yield return null;
        }

        Vector3 teleportPos = transform.position + direction * TPDist;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(teleportPos, out hit, 3f, NavMesh.AllAreas))
            navAgent.Warp(hit.position);

        canTeleport = true;
    }
    private void LimitedVisibiility()
    {
        //targetDistance = Vector3.Distance(navAgent.transform.position, target.transform.position);
        //if (!hasLimitedVisibility)
        //{
        //navAgent.SetDestination(target.transform.position);
        //}
        //if (targetDistance <= detectionRadius)
        //{
        //if (targetDistance <= attackDistance)
        //{
        //    //if (stopOnAttack)
        //    //{
        //    //    navAgent.isStopped = true;
        //    //    animator.SetBool("Attack", true);
        //    //}
        //}
        //else
        //{
        //if (stopOnAttack)
        //{
        //    navAgent.isStopped = false;
        //    animator.SetBool("Attack", false);
        //}
        //navAgent.SetDestination(target.transform.position);
    }

    void InitializeReferences()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (!navAgent) Debug.Log($"{transform.name} navAgent is null");
        navAgent.enabled = false;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            navAgent.enabled = true;
        }
        else
        {
            Debug.LogError($"{transform.name} Agent spawnPos too far from navmesh: " + hit.position);
        }
        this.enabled = true;
        navAgent.enabled = true;
        //animator = GetComponent<Animator>();
        if (!navAgent.isOnNavMesh)
        {
            Debug.LogError($"{transform.name} Agent not on Navmesh");
        }

        if (hasDash && hasTeleport)
        {
            Debug.LogError("cannot have two abilities");
            hasDash = false;
        }
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
    }
    void InitializeValues()
    {
        navAgent.speed = moveSpeed;
        navAgent.acceleration = acceleration;
    }
}


