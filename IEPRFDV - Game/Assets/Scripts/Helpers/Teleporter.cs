using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [Tooltip("Get panel from UI object")]
    [SerializeField] Countdown countdown;
    [SerializeField] int playersDetected;
    [SerializeField] private bool isCountingDown;

    private Coroutine countRoutine;

    private void Start()
    {
        playersDetected = 0;
        isCountingDown = false;
    }

    private void OnEnable()
    {
        countdown.OnCountdownFinished += TeleportPlayers;
    }

    private void OnDisable()
    {
        countdown.OnCountdownFinished -= TeleportPlayers;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playersDetected++;
        }

        if (playersDetected == 2 && !isCountingDown)
        {
            isCountingDown = true;
           // StartCoroutine(countdown.CountdownTo(5f));
            countdown.StartCountdown(5f);
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
            countdown.StopCountdown();
            //countdown.StopCountdown();
        }
    }

    private void TeleportPlayers()
    {
        StartCoroutine("TransitionToGameScene");
    }

    IEnumerator TransitionToGameScene()
    {
        yield return StartCoroutine(countdown.OnCountdownEnd("Teleporting...", 1f));
        countdown.StopCountdown();
        SceneManager.LoadScene("GameScene");
    }



}
