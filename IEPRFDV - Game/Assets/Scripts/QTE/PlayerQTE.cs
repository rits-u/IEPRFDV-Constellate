using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

public class PlayerQTE : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private RectTransform targetCircle;
    [SerializeField] private RectTransform movingCircle;

    [Header("Player Input")]
    [SerializeField] private int playerID;
    [SerializeField] private InputActionReference input;


    [Header("Properties")]
    [SerializeField] private bool onEnable;
    [SerializeField] private bool deactivateAfter;
    [SerializeField] private float growDuration = 1.5f;
    [SerializeField] private float startScale = 4f;
    [SerializeField] private float endScale = .5f;
    [SerializeField] private float tolerance = 0.35f;
    [SerializeField] private float restartCooldown = 1.5f;

    [Header("Interactions")]
    [SerializeField] private GameObject[] toActivateOnValid;
    [SerializeField] private GameObject[] toDeactivateOnValid;
    [SerializeField] private GameObject[] toActivateOnInvalid;
    [SerializeField] private GameObject[] toDeactivateOnInvalid;

    //[Header("Flags")]
    //[HideInInspector] private bool hasClicked = false;
    //[HideInInspector] private bool clickSuccess = false;
    //[HideInInspector] private bool activatedValidObjects = false;
    //[HideInInspector] private bool deactivatedValidObjects = false;
    //[HideInInspector] private bool activatedInvalidObjects = false;
    //[HideInInspector] private bool deactivatedInvalidObjects = false;

    [HideInInspector] private float time = 0f;

    private UserInterfaceQTE UI;

    private float cooldownTimer = 0f;
    private bool isCooldown = true;
    private bool resolved = false;

    private void Awake()
    {
        InitializeReferences();
        InitializeValues();
    }

    void Start()
    {
        if (onEnable) return;
    }

    private void OnEnable()
    {
        if (onEnable)
        {
            input.action.Enable();
            input.action.performed += OnInputPressed;

            UI = GetComponent<UserInterfaceQTE>();
            UI.HideHitResult();

            InitializeValues();
            StartCooldown();
        }
    }

    private void OnDisable()
    {
        input.action.Disable();
        input.action.performed -= OnInputPressed;
    }

    private void Update()
    {
        MoveCircle();
    }

    public void StartQTE()
    {
        gameObject.SetActive(true);
        InitializeValues();
        StartCooldown();
    }

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

    private void MoveCircle()
    {
        if (isCooldown)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= restartCooldown)
            {
                isCooldown = false;
                cooldownTimer = 0f;
                time = 0f;
                UI.HideHitResult();
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
            // ResetInputs();
        }
    }

    void OnInputPressed(InputAction.CallbackContext ctx)
    {
        if (resolved) return;

        float value = ctx.ReadValue<float>();
       // UI.PressFeedback(value);
        StartCoroutine(ResetKey(value));

        bool valid = IsAOverlapB(movingCircle, targetCircle);
        if (valid) UI.ShowFeedbackUI("Hit"); 
        else UI.ShowFeedbackUI("Miss");

        QTEManager.Instance.PlayerPressed(playerID, value, valid);
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

    public void ShowFeedbackUI(string name)
    {
        UI.ShowFeedbackUI(name);
    }

    public void HideResultsUIElements()
    {
        UI.HideHitResult();
        UI.HideResultVisual();
    }

    IEnumerator ResetKey(float value)
    {
        yield return new WaitForSeconds(0.25f);
        UI.ResetPressFeedback(value);
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

        targetCircle.gameObject.SetActive(true);
        movingCircle.gameObject.SetActive(true);

    }
    void InitializeValues()
    {

    }
    public void Deactivate()
    {
        time = 0f;
        //activatedValidObjects = false;
        //deactivatedValidObjects = false;
        //activatedInvalidObjects = false;
        //deactivatedInvalidObjects = false;
       // targetCircle.gameObject.SetActive(false);
       // movingCircle.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

}

