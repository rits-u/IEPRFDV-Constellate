using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum QTEResult
{
    Share,
    P1Steals,
    P2Steals,
    None
}

public class TestQTE : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] private string name;
    [SerializeField] private RectTransform targetCircle;
    [SerializeField] private RectTransform movingCircle;
        
    [Header("Player Input")]
    [SerializeField] private InputActionReference P1Input;
    [SerializeField] private InputActionReference P2Input;
    // [SerializeField] float inputWindow = 0.2f;
    //private float p1PressTime = -1f;
    //private float p2PressTime = -1f;

    [Header("Properties")]
    [SerializeField] private bool onEnable;
    [SerializeField] private bool deactivateAfter;
    //[SerializeField] private KeyCode inputKeySteal;
    //[SerializeField] private KeyCode inputKeyShare;

    [SerializeField] private float duration = 5f;
    [SerializeField] private float growDuration = 1.5f;
    [SerializeField] private float startScale = 4f;
    [SerializeField] private float endScale = .5f;
    [SerializeField] private float tolerance = 0.35f;
    [SerializeField] private float restartCooldown = 1.5f;

    private float cooldownTimer = 0f;
    private bool isCooldown = true;

    //[HideInInspector] private float timer;
    //[HideInInspector] private bool active;


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

    [Header("UI Elements")]
    [SerializeField] Sprite spriteHit;
    [SerializeField] Sprite spriteMiss;
    [SerializeField] Sprite spriteShare;
    [SerializeField] Sprite spriteSteal;
    [SerializeField] Image p1ResultImage;
    [SerializeField] Image p2ResultImage;


    [HideInInspector] private float time = 0f;

    public event Action OnFinished;

    private float p1Value, p2Value;
    private bool p1Valid, p2Valid;

    private float p1Time = -1;
    private float p2Time = -1;


    private float bufferWindow = 0.2f;

    private bool resolved = false;



    private void Awake()
    {
        InitializeReferences();
        InitializeValues();
    }
    void Start()
    {
        if (onEnable) return;

        //StartCoroutine(RunTimer());
    }

    private void OnEnable()
    {
        if (onEnable)
        {
            P1Input.action.Enable();
            P2Input.action.Enable();

            P1Input.action.performed += OnP1Pressed;
            P2Input.action.performed += OnP2Pressed;

            InitializeValues();
            //   StartCoroutine(RunTimer());
            StartCooldown();
        }
    }

    private void OnDisable()
    {
        P1Input.action.Disable();
        P2Input.action.Disable();

        P1Input.action.performed -= OnP1Pressed;
        P2Input.action.performed -= OnP2Pressed;
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
        //if (CheckKeyBindPress())
        //{
        //    HandleClickSuccess();

        //    if (deactivateAfter)
        //    {
        //        Debug.Log(name + ": deactivating");
        //        Deactivate();
        //    }
        //}

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

    //IEnumerator RunTimer()
    //{
    //    float timer = 0f;
    //    while (timer < duration)
    //    {
    //        if (hasClicked)
    //        {
    //            Debug.Log(name + ": Action completed in Time");
    //            FinishQTE();
    //            yield break;
    //        }
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }
    //    Debug.Log(name + ": time ran out");
    //    if (deactivateAfter)
    //    {
    //        Debug.Log(name + ": deactivating");
    //        //  Deactivate();
    //        FinishQTE();
    //    }
    //    // FinishQTE();

    //}

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

                HideInputSprite(p1ResultImage);
                HideInputSprite(p2ResultImage);
            }

            return;
        }

        time += Time.deltaTime;

        float t = Mathf.Clamp01(time / growDuration);
        float scale = Mathf.Lerp(startScale, endScale, t);

        movingCircle.localScale = Vector3.one * scale;

        if (t >= 1f)
        {
            movingCircle.localScale = Vector3.one * endScale;
            StartCooldown();
            ResetInputs();
        }
    }

    private void HideInputSprite(Image image)
    {
        image.sprite = null;
        Color temp = image.color;
        temp.a = 0f;
        image.color = temp;
    }

    private void ShowInputSprite(Image image, Sprite sprite)
    {
        Color temp = image.color;
        temp.a = 1f;
        image.color = temp;
        image.sprite = sprite;
    }

    //bool CheckKeyPress()
    //{
    //    if (Input.GetKeyDown(inputKeySteal) || Input.GetKeyDown(inputKeyShare))
    //    {
    //        hasClicked = true;
    //        if (IsAOverlapB(movingCircle, targetCircle))
    //        {
    //            Debug.Log(name + ": is inside area");
    //            clickSuccess = true;
    //            activatedValidObjects = SetObjects(toActivateOnValid, true);
    //            deactivatedValidObjects = SetObjects(toDeactivateOnValid, false);
    //        }
    //        else
    //        {
    //            Debug.Log(name + ": is outside area");
    //            activatedInvalidObjects = SetObjects(toActivateOnInvalid, true);
    //            deactivatedInvalidObjects = SetObjects(toDeactivateOnInvalid, false);
    //        }
    //        return true;
    //    }
    //    return false;
    //}


    void OnP1Pressed(InputAction.CallbackContext ctx)
    {
        if (resolved) return;

        p1Value = ctx.ReadValue<float>();
        p1Time = Time.time;

        p1Valid = IsAOverlapB(movingCircle, targetCircle);
        if (p1Valid) ShowInputSprite(p1ResultImage, spriteHit);
        else ShowInputSprite(p1ResultImage, spriteMiss);

        TryResolve();
    }

    void OnP2Pressed(InputAction.CallbackContext ctx)
    {
        if (resolved) return;

        p2Value = ctx.ReadValue<float>();
        p2Time = Time.time;
        p2Valid = IsAOverlapB(movingCircle, targetCircle);
        if (p2Valid) ShowInputSprite(p2ResultImage, spriteHit);
        else ShowInputSprite(p2ResultImage, spriteMiss);

        TryResolve();
    }

    void TryResolve()
    {
        if (p1Time < 0 || p2Time < 0)
            return;

        if (Mathf.Abs(p1Time - p2Time) <= bufferWindow)
        {
            if (p1Valid && p2Valid)
            {
                ResolveInputs();
            }
            else
            {
                Debug.Log("One player pressed outside the zone");
            }
        }
    }
    void ResolveInputs()
    {
        resolved = true;

        if (IsAOverlapB(movingCircle, targetCircle))
        {
            Debug.Log($"P1: {p1Value}  P2: {p2Value}");
            //    int result = 0;
            QTEResult result = QTEResult.None;

            if (p1Value > 0.5f && p2Value < -0.5f)
            {
                Debug.Log("share");
                result = QTEResult.Share;
            }
            else if (p1Value < -0.5f && p2Value < -0.5f)
            {
                Debug.Log("p1 steals");
                result = QTEResult.P1Steals;
            }
            else if (p1Value > 0.5f && p2Value > 0.5f)
            {
                Debug.Log("p2 steals");
                result = QTEResult.P2Steals;
            }
            else if (p1Value < -0.5f && p2Value > 0.5f)
            {
                result = QTEResult.None;
                Debug.Log("no one gets rewards");
            }

            StartCoroutine(ProcessResults(result));

            clickSuccess = true;

            //if (deactivateAfter)
            //    Deactivate();
        }
        else
        {
            Debug.Log("outside zone");
        }

        HandleClickSuccess();

        //if (deactivateAfter)
        //    Deactivate();
    }

    void ResetInputs()
    {
        p1Time = -1;
        p2Time = -1;

        p1Value = 0;
        p2Value = 0;

        resolved = false;
    }

    IEnumerator ProcessResults(QTEResult result)
    {
        yield return new WaitForSeconds(1.0f);
        if (deactivateAfter)
            Deactivate();

        switch (result)
        {
            case QTEResult.Share:
                ShowInputSprite(p1ResultImage, spriteShare);
                ShowInputSprite(p2ResultImage, spriteShare);
                break;
            case QTEResult.P1Steals:
                ShowInputSprite(p1ResultImage, spriteSteal);
                ShowInputSprite(p2ResultImage, spriteShare);
                break;
            case QTEResult.P2Steals:
                ShowInputSprite(p1ResultImage, spriteShare);
                ShowInputSprite(p2ResultImage, spriteSteal);
                break;
            case QTEResult.None:
                ShowInputSprite(p1ResultImage, spriteSteal);
                ShowInputSprite(p2ResultImage, spriteSteal);
                break;
        }

//        LootManager.Instance.ResolveLoot(result);

        yield return new WaitForSeconds(2.0f);
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
