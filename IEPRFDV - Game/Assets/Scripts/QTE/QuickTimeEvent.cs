using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class QuickTimeEvent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform targetCircle;
    [SerializeField] private RectTransform movingCircle;
    [HideInInspector] private RectTransform parent;

    [HideInInspector] private string name;
    [HideInInspector] private float xPos;

    [Header("Properties")]
    [SerializeField] private bool onEnable;
    [SerializeField] private bool deactivateAfter;
    [SerializeField] private KeyCode inputKeySteal;
    [SerializeField] private KeyCode inputKeyShare;

    [SerializeField] private float duration = 5f;
    [SerializeField] private float growDuration = 1.5f;
    [SerializeField] private float startScale = 4f;
    [SerializeField] private float endScale = .5f;
    [SerializeField] private float tolerance = 0.35f;

    [HideInInspector] private float timer;
    [HideInInspector] private bool active;


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

        StartCoroutine(RunTimer());
    }

    private void OnEnable()
    {
        if (onEnable)
        {
            InitializeValues();
            StartCoroutine(RunTimer());
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
        StartCoroutine(RunTimer());
    }

    public IEnumerator  PlayQTE(GameObject player)
    {
        switch (player.name)
        {
            case "Player 1":
                inputKeySteal = KeyCode.W;
                inputKeyShare = KeyCode.S;
                transform.position = new Vector2(xPos, transform.position.y);
                break;
            case "Player 2":
                inputKeySteal = KeyCode.UpArrow;
                inputKeyShare = KeyCode.DownArrow;
                transform.position = new Vector2(xPos * 2, transform.position.y);
                break;
            default:
                inputKeySteal = KeyCode.W;
                inputKeyShare = KeyCode.S;
                transform.position = new Vector2(xPos, transform.position.y);
                break;
        }
        bool finished = false;

        System.Action handler = () => finished = true; //event thatll mark qte finished
        OnFinished += handler;

        //reset
        hasClicked = false;
        clickSuccess = false;
        InitializeValues();
        gameObject.SetActive(true);


        StartCoroutine(RunTimerEvent());

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

    IEnumerator RunTimerEvent()
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
        if (deactivateAfter)
        {
            Debug.Log(name + ": time ran out，deactivating");
            Deactivate();
        }
    }

    IEnumerator RunTimer()
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
        time += Time.deltaTime;
        float t = Mathf.PingPong(time / growDuration, 1f);

        float scale = Mathf.Lerp(startScale, endScale, t);
        movingCircle.localScale = Vector3.one * scale;

    }

    bool CheckKeyPress()
    {
        if (Input.GetKeyDown(inputKeySteal) || Input.GetKeyDown(inputKeyShare))
        {
            hasClicked = true;
            if (IsAOverlapB(movingCircle, targetCircle))
            {
                Debug.Log(name + ": is inside area");
                clickSuccess = true;
                activatedValidObjects = SetObjects(toActivateOnValid, true);
                deactivatedValidObjects = SetObjects(toDeactivateOnValid, false);
            }
            else
            {
                Debug.Log(name + ": is outside area");
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

    bool IsAOverlapB(RectTransform a, RectTransform b)
    {
        float currentScale = a.localScale.x;
        float targetScale = b.localScale.x;

        //float debug = MathF.Abs(currentScale - targetScale);
        //Debug.Log($"{name} {debug} || {currentScale} - {targetScale} <= tolerance? {MathF.Abs(currentScale - targetScale) <= tolerance}");
        if (MathF.Abs(currentScale - targetScale) <= tolerance)
        {
            return true;
        }
        return false;
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

   //void CreateChildRect(string name, RectTransform rect, Vector3 pos)
   // {
   //     GameObject child = new GameObject(name);
   //     child.name = name;
   //     child.transform.SetParent(transform);

   //     rect = child.AddComponent<RectTransform>();
   //     rect.localScale = Vector3.one;
   //     rect.position = pos;
   // }

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
        if (!targetCircle)
        {
            Debug.LogError($"{gameObject.name}'s targetCircle is null");
        }
        if (!movingCircle)
        {
            Debug.LogError($"{gameObject.name}'s movingCircle is null");
        }

        if (inputKeySteal == KeyCode.None)
        {
            inputKeySteal = KeyCode.W;
            //Debug.LogError($"{gameObject.name}'s inputKeySteal is null");
        }
        if (inputKeyShare == KeyCode.None)
        {
            inputKeyShare = KeyCode.S;
            //Debug.LogError($"{gameObject.name}'s inputKeyShare is null");
        }
        parent = transform.parent.GetComponent<RectTransform>();
    }
    void InitializeValues()
    {
        xPos = parent.rect.width * 0.3333f;
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
