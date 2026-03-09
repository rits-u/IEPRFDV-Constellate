using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

public class PlayerQTE : MonoBehaviour
{

    [Header("References")]
    [HideInInspector] private string name;
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

    private float cooldownTimer = 0f;
    private bool isCooldown = true;

    [Header("UI Elements")]
    [SerializeField] Sprite spriteHit;
    [SerializeField] Sprite spriteMiss;
    [SerializeField] Image resultImage;

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

   // public event Action OnPressed;
    public event Action<PlayerQTE> OnPressed;

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
            input.action.Enable();
            input.action.performed += OnInputPressed;

            InitializeValues();
            //   StartCoroutine(RunTimer());
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
        // StartCoroutine(RunTimer());
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
                hasClicked = false;

                HideSprite();
                HideSprite();
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

    public void HideSprite()
    {
        resultImage.sprite = null;
        Color temp = resultImage.color;
        temp.a = 0f;
        resultImage.color = temp;
    }

    public void ShowSprite(Sprite sprite)
    {
        Color temp = resultImage.color;
        temp.a = 1f;
        resultImage.color = temp;
        resultImage.sprite = sprite;
    }


    void OnInputPressed(InputAction.CallbackContext ctx)
    {
        if (resolved) return;

        float value = ctx.ReadValue<float>();
     //   p1Time = Time.time;

        bool valid = IsAOverlapB(movingCircle, targetCircle);
        if (valid) ShowSprite(spriteHit);
        else ShowSprite(spriteMiss);

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

    //void ResetInputs()
    //{
    //    p1Time = -1;
    //    p2Time = -1;

    //    p1Value = 0;
    //    p2Value = 0;

    //    resolved = false;
    //}



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
    public void Deactivate()
    {
        time = 0f;
        activatedValidObjects = false;
        deactivatedValidObjects = false;
        activatedInvalidObjects = false;
        deactivatedInvalidObjects = false;
        gameObject.SetActive(false);
    }

}
