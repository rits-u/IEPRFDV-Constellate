using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using System;

public class QuickTimeEvent : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] private string name;
    [SerializeField] private RectTransform ball;
    [SerializeField] private RectTransform safeZone;
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;

    [Header("Properties")]
    [SerializeField] private bool onEnable;
    [SerializeField] private bool deactivateAfter;
    [SerializeField] private KeyCode inputKey1;
    [SerializeField] private KeyCode inputKey2;

    [SerializeField] private float maxTime;
    [SerializeField] private float AToBTime;
    [HideInInspector] private Vector3 startPos;
    

    [Header("Interactions")]
    [SerializeField] private GameObject[] toActivateOnValid;
    [SerializeField] private GameObject[] toDeactivateOnValid;
    [SerializeField] private GameObject[] toActivateOnInvalid;
    [SerializeField] private GameObject[] toDeactivateOnInvalid;

    [Header("Flags")]
    [HideInInspector] private bool hasClicked = false;
    [HideInInspector] private bool clickSuccess = false;
    [HideInInspector] private bool activatedValidObjects = false;
    [HideInInspector] private bool deactivatedValidObjects = false;
    [HideInInspector] private bool activatedInvalidObjects = false;
    [HideInInspector] private bool deactivatedInvalidObjects = false;


    [HideInInspector] private float time = 0f;

    public event Action OnFinished;

    private void Awake()
    {
        InitializeReferences();
        InitializeValues();
    }
    void Start()
    {
        if (onEnable) return;

        StartCoroutine(RunTimer(maxTime));
    }

    private void OnEnable()
    {
        if (onEnable)
        {
            InitializeValues();
            StartCoroutine(RunTimer(maxTime));
        }
    }

    void Update()
    {
        MoveBall();
        if (CheckKeyPress())
        {
            HandleClickSuccess();

            if (deactivateAfter)
            {
                Debug.Log(name + ": deactivating");
                Deactivate();
            }
        }
    }

    public void StartQTE()
    {
        gameObject.SetActive(true);
        InitializeValues();          
        StartCoroutine(RunTimer(maxTime));
    }

    public IEnumerator PlayQTE()
    {
        bool finished = false;

        System.Action handler = () => finished = true; //event thatll mark qte finished
        OnFinished += handler;

        //reset
        hasClicked = false;
        clickSuccess = false;
        InitializeValues();
        gameObject.SetActive(true);

      
        StartCoroutine(RunTimerEvent(maxTime));

        //wait until player clicks or timer runs out
        yield return new WaitUntil(() => finished);

        //unsubscribe
        OnFinished -= handler;
        Deactivate();
    }

    private void FinishQTE()
    {

        if (!gameObject.activeSelf) return;

       // Debug.Log("qte done");
        OnFinished?.Invoke();
       // Deactivate();
    }

    IEnumerator RunTimerEvent(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (hasClicked)
            {
                Debug.Log(name + ": Action completed in Time");
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log(name + ": time ran out");
        if (deactivateAfter)
        {
            Debug.Log(name + ": deactivating");
            Deactivate();
        }
    }

    IEnumerator RunTimer(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (hasClicked)
            {
                Debug.Log(name + ": Action completed in Time");
                FinishQTE();
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log(name + ": time ran out");
        if (deactivateAfter)
        {
            Debug.Log(name + ": deactivating");
          //  Deactivate();
            FinishQTE();
        }
       // FinishQTE();

    }

    void MoveBall()
    {
        Vector3 offset = pointB.transform.position - pointA.transform.position;
        time += Time.deltaTime;
        float t = Mathf.PingPong(time / AToBTime, 1f);
        //ball.position = startPos + Vector3.Lerp(Vector3.zero, pointB.transform.position, t);
        ball.position = Vector3.Lerp(pointA.transform.position, pointB.transform.position, t);
    }

    bool CheckKeyPress()
    {
        if (Input.GetKeyDown(inputKey1)|| Input.GetKeyDown(inputKey2))
        {
            hasClicked = true;
            if (IsOverlap(ball, safeZone))
            {
                Debug.Log(name + ": ball inside area");
                clickSuccess = true;
                activatedValidObjects = SetObjects(toActivateOnValid, true);
                deactivatedValidObjects = SetObjects(toDeactivateOnValid, false);
            }
            else
            {
                Debug.Log(name + ": ball outside area");
                activatedInvalidObjects = SetObjects(toActivateOnInvalid, true);
                deactivatedInvalidObjects = SetObjects(toDeactivateOnInvalid, false);
            }
            return true;
        }
        return false;
    }
    void HandleClickSuccess()
    {
        if (!clickSuccess)
        {
            if (toActivateOnInvalid.Length != 0 && !activatedInvalidObjects)
                SetObjects(toActivateOnInvalid, true);
            if (toDeactivateOnInvalid.Length != 0 && !deactivatedInvalidObjects)
                SetObjects(toDeactivateOnInvalid, false);
        }
    }

    bool IsOverlap(RectTransform a, RectTransform b)
    {
        Rect rectA = GetWorldRect(a);
        Rect rectB = GetWorldRect(b);
        return rectA.Overlaps(rectB);
    }

    bool SetObjects(GameObject[] objects, bool value)
    {
        foreach (GameObject objs in objects)
        {
            if (objs == null) continue;
            objs.SetActive(true);
        }
        return true;
    }

    Rect GetWorldRect(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];

        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }
    void InitializeReferences()
    {
        if (name == null || name.Length == 0)
        {
            name = gameObject.name;
        }
        if (!ball)
        {
            Debug.LogError("ball is null");
        }
        if (!pointA)
        {
            Debug.LogError("pointA is null");
        }
        if (!pointB)
        {
            Debug.LogError("pointB is null");
        }
        if (!safeZone)
        {
            Debug.LogError("safeZone is null");
        }
        if (AToBTime == 0)
        {
            AToBTime = 1;
        }
    }
    void InitializeValues()
    {
        startPos = ball.transform.position;
    }
    void Deactivate()
    {
        time = 0f;
        activatedValidObjects = false;
        deactivatedValidObjects = false;
        activatedInvalidObjects = false;
        deactivatedInvalidObjects = false;
        gameObject.SetActive(false);
    }
}
