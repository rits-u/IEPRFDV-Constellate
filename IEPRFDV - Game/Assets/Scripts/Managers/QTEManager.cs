using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("Player QTEs")]
    [SerializeField] private PlayerQTE p1QTE;
    [SerializeField] private PlayerQTE p2QTE;

    [Header("Properties")]
    [SerializeField] float bufferWindow = 0.2f;

    [Header("Fields")]
    private float p1Time = -1, p2Time = -1;
    [SerializeField] private float p1Value, p2Value;
    [SerializeField] private bool p1Valid, p2Valid;
    [SerializeField] private bool resolving = false;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button continueBtn;


    public void PlayerPressed(int playerID, float value, bool valid)
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
            //Debug.Log("try resolve");
            Resolve();
        }
    }

    IEnumerator BufferTimer()
    {
        yield return new WaitForSeconds(bufferWindow);

        //if (!resolving)
        //{
        //    Debug.Log("just resolve");
        //    Resolve();
        //}
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

            SetResultSprites(result);
            LootManager.Instance.ResolveLoot(result);
            StartCoroutine(EnableContinueButton());
            DeactivateAll();
         //   resultText.text = "";
        }

        resolving = false;
    }

    void SetResultSprites(QTEResult result)
    {
        switch (result)
        {
            case QTEResult.Share:
                p1QTE.ShowFeedbackUI("Share");
                p2QTE.ShowFeedbackUI("Share");
                resultText.text = "Each player will share the loot and will get Tier 1 rewards.";
                break;
            case QTEResult.P1Steals:
                p1QTE.ShowFeedbackUI("Steal");
                p2QTE.ShowFeedbackUI("Share");
                resultText.text = "Player 1 has stolen and got Tier 2 rewards! Player 2 gets none.";
                break;
            case QTEResult.P2Steals:
                p1QTE.ShowFeedbackUI("Share");
                p2QTE.ShowFeedbackUI("Steal");
                resultText.text = "Player 2 has stolen and got Tier 2 rewards! Player 1 gets none.";
                break;
            case QTEResult.None:
                p1QTE.ShowFeedbackUI("Steal");
                p2QTE.ShowFeedbackUI("Steal");
                resultText.text = "Both attempted to steal, no rewards will be given for this round.";
                break;
        }
    }

    private IEnumerator EnableContinueButton()
    {
        yield return new WaitForSeconds(3f);
        continueBtn.gameObject.SetActive(true);
    }

    public void HideResults()
    {
        p1QTE.HideResultsUIElements();
        p2QTE.HideResultsUIElements();
        resultText.text = "";
    }

    private void DeactivateAll()
    {
        p1QTE.Deactivate();
        p2QTE.Deactivate();
    }
}
