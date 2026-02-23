using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AI_FollowPlayer : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject[] players;
    private GameObject target = null;

    [Header("Movement")]
    //[SerializeField] private float moveSpeed = 0.8f;
    //[SerializeField] private bool randomMovement = false;
    //[SerializeField] private bool hasLimitedVisibility = false;
    //[ShowIf("hasLimitedVisibility")][SerializeField] private float detectionRadius = 5.0f;
    //[ShowIf("randomMovement")][SerializeField] private float randomMovementRange = 5.0f;
    [SerializeField] private float pathRefreshTime = 3.0f;


    [Header("Attack")]
    [SerializeField] private bool stopOnAttack = false;
    //[SerializeField] private float cooldown;
    
    private Vector3 positionOffset;
    private NavMeshAgent navAgent;
    //private Animator animator;
    //private float detectionBuffer = 1.0f;
    private float targetDistance;

    void Start()
    {
        if (players == null || players.Length == 0)
        {
            Debug.Log("no players");
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        target = players[0];

        this.enabled = true;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = true;
        //animator = GetComponent<Animator>();

        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
        //navAgent.speed = moveSpeed;

        //if (attackDistance < detectionRadius)
        //{
        //    detectionRadius = attackDistance + 3.0f;
        //}

        target.GetComponent<Stats>().OnDeath += OnTargetDeath;

        SetRotation();
        StartCoroutine(WaitTimer());
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
    void LateUpdate()
    {
        if (target == null) return;

        //targetDistance = Vector3.Distance(navAgent.transform.position, target.transform.position);
        SetRotation();

        //if (!hasLimitedVisibility)
        //{
            navAgent.SetDestination(target.transform.position);
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
        float shortest = Mathf.Infinity;
        GameObject toFollow = null;
        foreach (GameObject player in players)
        {
            if (!player) continue;

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
            currentTargetStats = target.GetComponent<Stats>();
            currentTargetStats.OnDeath += OnTargetDeath;
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
}


