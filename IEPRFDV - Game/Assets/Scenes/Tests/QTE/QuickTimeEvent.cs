using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class QuickTimeEvent : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] private string name;
    [SerializeField] private RectTransform targetCircle;
    [SerializeField] private RectTransform movingCircle;

    [Header("Player Input")]
    [SerializeField] private InputActionReference P1Input;
    //[SerializeField] private InputActionReference P2Input;

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
    [SerializeField] private float restartCooldown = 1.5f;

    private float cooldownTimer = 0f;
    private bool isCooldown = true;

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

        P1Input.action.Enable();
        //StartCoroutine(RunTimer());
    }

    private void OnEnable()
    {
        if (onEnable)
        {
            P1Input.action.Enable();
            InitializeValues();
         //   StartCoroutine(RunTimer());
            StartCooldown();
        }
    }

    void Update()
    {
        MoveCircle();
        //if (CheckKeyPress())
        //{
        //    HandleClickSuccess();

        //    if (deactivateAfter)
        //    {
        //        Debug.Log(name + ": deactivating");
        //        Deactivate();
        //    }
        //}
        if (CheckKeyBindPress())
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
        StartCooldown();
        // StartCoroutine(RunTimer());
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

    //void MoveBall()
    //{
    //    time += Time.deltaTime;
    //    float t = Mathf.PingPong(time / growDuration, 1f);

    //    float scale = Mathf.Lerp(startScale, endScale, t);
    //    movingCircle.localScale = Vector3.one * scale;

    //}

    void StartCooldown()
    {
        ResetCircle();
        isCooldown = true;
        cooldownTimer = 0f;
    }

    void ResetCircle()
    {
        movingCircle.localScale = Vector3.one * startScale;
    }

    void MoveCircle()
    {
        if (isCooldown)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= restartCooldown)
            {
                isCooldown = false;
                cooldownTimer = 0f;
                time = 0f;
                hasClicked = false;
            }

            return;
        }

        time += Time.deltaTime;

        float t = time / growDuration;
        float scale = Mathf.Lerp(startScale, endScale, t);
        movingCircle.localScale = Vector3.one * scale;

       
        if (t >= 1f)
        {
            StartCooldown();
        }
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


    bool CheckKeyBindPress()
    {
        float P1_qte = P1Input.action.ReadValue<float>();

        if (P1Input.action.WasPressedThisFrame() && !hasClicked)
        {
            hasClicked = true;

            if (IsAOverlapB(movingCircle, targetCircle))
            {
                Debug.Log(name + ": inside area");

                if (P1_qte == 1.0) Debug.Log("Share <3");
                clickSuccess = true;
            }
            else
            {
                Debug.Log(name + ": outside area");
            }

            return true;
        }

        return false;
    }

    //bool CheckKeyBindPress()
    //{
    //    float P1_qte = P1Input.action.ReadValue<float>();
    //    float P2_qte = P2Input.action.ReadValue<float>();
    //   // Debug.Log($"player qte: {P1_qte}");

    //    if (P1_qte == 1.0f || P2_qte == 1.0f) 
    //    {
    //        hasClicked = true;
    //        if (IsOverlap(ball, safeZone))
    //        {
    //            Debug.Log(name + ": ball inside area");
    //            clickSuccess = true;
    //            activatedValidObjects = SetObjects(toActivateOnValid, true);
    //            deactivatedValidObjects = SetObjects(toDeactivateOnValid, false);
    //        }
    //        else
    //        {
    //            Debug.Log(name + ": ball outside area");
    //            activatedInvalidObjects = SetObjects(toActivateOnInvalid, true);
    //            deactivatedInvalidObjects = SetObjects(toDeactivateOnInvalid, false);
    //        }
    //        return true;
    //    }
    //    return false;
    //}



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

        //if (inputKeySteal == KeyCode.None)
        //{
        //    Debug.LogError($"{gameObject.name}'s inputKeySteal is null");
        //}
        //if (inputKeyShare == KeyCode.None)
        //{
        //    Debug.LogError($"{gameObject.name}'s inputKeyShare is null");
        //}

    }
    void InitializeValues()
    {

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
