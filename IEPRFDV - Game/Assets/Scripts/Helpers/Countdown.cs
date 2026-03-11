using TMPro;
using UnityEngine;
using System.Collections;
using System;

public class Countdown : MonoBehaviour
{
    private TextMeshProUGUI countdownText;
    private Coroutine countdownRoutine;

    public event Action OnCountdownFinished;

    private void Start()
    {
        countdownText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void StartCountdown(float duration, string startingText)
    {
        countdownRoutine = StartCoroutine(CountdownTo(duration, startingText));
    }

    public IEnumerator CountdownTo(float duration, string startingText)
    {
        if (countdownText == null)
        {
            Debug.Log("Countdown Text is missing");
            yield break;
        }

        float countdown = duration;

        while (countdown > 0)
        {
            countdownText.text = startingText + Mathf.CeilToInt(countdown).ToString();
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        countdownText.text = "";
        OnCountdownFinished?.Invoke();
    }

    public void StopCountdown()
    {
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }
        ResetCountdown();
    }

    public void ResetCountdown()
    {
        countdownText.text = "";
    }

    public IEnumerator OnCountdownEnd(string text, float waitTime)
    {
        countdownText.text = text;
        yield return new WaitForSeconds(waitTime);
    }
}
