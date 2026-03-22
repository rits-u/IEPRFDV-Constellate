using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RoundTriggerArea : MonoBehaviour
{
    [Tooltip("Get panel from UI object")]
    [SerializeField] Countdown countdown;
    [SerializeField] Countdown titleCountdown;

    [SerializeField] private float countdownDuration;
    [SerializeField] private int playersDetected;
    [SerializeField] private bool isCountingDown;

    private void Start()
    {
        playersDetected = 0;
        isCountingDown = false;
    }

    private void OnEnable()
    {
        countdown.OnCountdownFinished += PrepareRound;
        titleCountdown.OnCountdownFinished += PrepareRound;
    }

    private void OnDisable()
    {
        countdown.OnCountdownFinished -= PrepareRound;
        titleCountdown.OnCountdownFinished -= PrepareRound;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playersDetected++;
        }

        if (playersDetected == 2 && !isCountingDown)
        {
            isCountingDown = true;
            // StartCoroutine(countdown.CountdownTo(5f));
            if(RoundManager.Instance.RoundNumber == 1) 
                titleCountdown.StartCountdown(countdownDuration, "Starting in... ");
            else 
                countdown.StartCountdown(countdownDuration, "Starting in... ");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playersDetected--;
        }

        if (playersDetected <= 1 && isCountingDown)
        {
            isCountingDown = false;

            if (RoundManager.Instance.RoundNumber == 1)
                titleCountdown.StopCountdown();
            else 
                countdown.StopCountdown();
            //countdown.StopCountdown();
        }
    }

    private void PrepareRound()
    {
        StartCoroutine(StartRoundProper());
    }

    IEnumerator StartRoundProper()
    {
        if (PlayerManager.Instance != null) PlayerManager.Instance.DisableAllPlayerMovement();
        if (RoundManager.Instance.RoundNumber == 1)
        {
            yield return StartCoroutine(titleCountdown.OnCountdownEnd("Starting Round...", 1f));
            titleCountdown.StopCountdown();
        }
        else
        {
            yield return StartCoroutine(countdown.OnCountdownEnd("Starting Round...", 1f));
            countdown.StopCountdown();
        }

        RoundManager.Instance.ExecuteRound();
    }

}
