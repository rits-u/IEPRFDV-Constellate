using UnityEditorInternal;
using UnityEngine;
using System.Collections;

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private PlayerQTE p1QTE;
    [SerializeField] private PlayerQTE p2QTE;
    //private QTEUserInterface ; 

    [SerializeField] float bufferWindow = 0.2f;

    [Header("UI Elements")]
    [SerializeField] private Sprite spriteShare;
    [SerializeField] private Sprite spriteSteal;

    private float p1Time = -1;
    private float p2Time = -1;

    private float p1Value;
    private float p2Value;

    private bool p1Valid;
    private bool p2Valid;

    private bool p1Ready;
    private bool p2Ready;

    private bool resolving = false;

    //private void Start()
    //{
    //    p1QTE.OnPressed += OnPlayerPressed;
    //    p2QTE.OnPressed += OnPlayerPressed;
    //}

    public void OnPlayerPressed(int playerID, float value, bool valid)
    {
        if (resolving) return;

        float pressTime = Time.time;

        if (playerID == 1)
        {
            p1Time = pressTime;
            p1Value = value;
            p1Valid = valid;
        }
        else
        {
            p2Time = pressTime;
            p2Value = value;
            p2Valid = valid;
        }

        TryResolve();
    }

    void TryResolve()
    {
        if (p1Time < 0 || p2Time < 0)
        {
            
            StartCoroutine(BufferTimer());
            return;
        }

        if (Mathf.Abs(p1Time - p2Time) <= bufferWindow)
        {
            Resolve();
        }
    }

    IEnumerator BufferTimer()
    {
        yield return new WaitForSeconds(bufferWindow);

        if (!resolving)
        {
            Resolve();
        }
    }

    void Resolve()
    {
        resolving = true;

        QTEResult result = QTEResult.None;

        if (p1Valid && p2Valid)
        {
            if (p1Value > 0.5f && p2Value < -0.5f)
                result = QTEResult.Share;

            else if (p1Value < -0.5f && p2Value < -0.5f)
                result = QTEResult.P1Steals;

            else if (p1Value > 0.5f && p2Value > 0.5f)
                result = QTEResult.P2Steals;
        }

        Debug.Log($"Result: {result}");
        SetResultSprites(result);
        LootManager.Instance.ResolveLoot(result);
        DeactivateAll();
    }

    //private void HideInputSprite(Image image)
    //{
    //    image.sprite = null;
    //    Color temp = image.color;
    //    temp.a = 0f;
    //    image.color = temp;
    //}

    //private void ShowInputSprite(Image image, Sprite sprite)
    //{
    //    Color temp = image.color;
    //    temp.a = 1f;
    //    image.color = temp;
    //    image.sprite = sprite;
    //}


    void SetResultSprites(QTEResult result)
    {
        switch (result)
        {
            case QTEResult.Share:
                p1QTE.ShowSprite(spriteShare);
                p2QTE.ShowSprite(spriteShare);
                break;

            case QTEResult.P1Steals:
                p1QTE.ShowSprite(spriteSteal);
                p2QTE.ShowSprite(spriteShare);
                break;

            case QTEResult.P2Steals:
                p1QTE.ShowSprite(spriteShare);
                p2QTE.ShowSprite(spriteSteal);
                break;

            case QTEResult.None:
                p1QTE.ShowSprite(spriteSteal);
                p2QTE.ShowSprite(spriteSteal);
                break;
        }
    }

    private void DeactivateAll()
    {
        p1QTE.Deactivate();
        p2QTE.Deactivate();
    }
}
