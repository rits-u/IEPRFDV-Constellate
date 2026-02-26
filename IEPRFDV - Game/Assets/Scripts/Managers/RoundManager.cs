using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [SerializeField] private int round;
    [SerializeField] int roundDuration;
    [SerializeField] private float countdownDuration = 3;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI roundNumberText;
    [SerializeField] private TextMeshProUGUI roundDurationText;

    [Header("QTE")]
    [SerializeField] private QuickTimeEvent chestQTE;

    //private float countdown;

    private bool roundEnded = false;

    public int RoundNumber
    {
        get => round;
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //CountdownToStart();
    }


    public void CountdownToStart()
    {
        // countdownPanel.SetActive(true);
        //StartCoroutine(Countdown());
        StartCoroutine(RoundFlow());

    }

    IEnumerator Countdown()
    {
        float countdown = countdownDuration;
       
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown -= 1;
        }

      //  countdownText.text = "START!";
       
        countdownText.text = "";
        //ExecuteRound();
    }

    IEnumerator Countdown(float duration, TextMeshProUGUI textUI)
    {
        float countdown = duration;

        while (countdown > 0)
        {
            textUI.text = Mathf.CeilToInt(countdown).ToString();
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        textUI.text = "";
    }

    IEnumerator RoundTime()
    {
        roundEnded = false;
        int timer = roundDuration;
        int secondsCount = timer;
        int minutesCount = timer / 60;
        //  if(roundDuration > 60) 

        string minutes = "";
        string seconds = "";

        SpawnManager.Instance.StartSpawning();

        while(timer > 0)
        {
            secondsCount = timer - minutesCount * 60;
            minutes = minutesCount.ToString();
            seconds = secondsCount.ToString();

            if (secondsCount % 60 == 0) seconds = "00";
            else if(secondsCount < 10) seconds = "0" + secondsCount.ToString();

            roundDurationText.text = minutes + ":" + seconds;

            if (secondsCount % 60 == 0) minutesCount -= 1;

//            Debug.Log(timer);
            yield return new WaitForSeconds(1f);
            timer -= 1;
            
        }

        SpawnManager.Instance.StopSpawning();
        //EnemyManager.Instance.UnregisterAllEnemies();

        roundDurationText.text = "";
        roundEnded = true;
    }

    public void ExecuteRound()
    {
        StartCoroutine(RoundTime());

    }

    //private IEnumerator RoundFlow()
    //{
    //    yield return StartCoroutine(Countdown());
    //    yield return StartCoroutine(Countdown(3f, countdownText));
    //    ExecuteRound();

    //    yield return StartCoroutine(RoundTime());

    //    EnemyManager.Instance.DestroyAllEnemies();

    //    yield return new WaitForSeconds(1);
    //    round timer
    //    yield return StartCoroutine(Countdown(5f, countdownText));

    //    chestQTE.StartQTE();

    //    adjust enemy


    //    StartCoroutine(RoundFlow());


    //    losing condition, exit loop when a player's HP reaches 0
    //}

    private IEnumerator RoundFlow()
    {
        //have !gameOver condition
        while (IsGameRunning())
        {
            roundNumberText.text = "Round: " + round;

            //yield return StartCoroutine(Countdown());
            PlayerManager.Instance.EnableAllPlayerMovement();
            yield return StartCoroutine(Countdown(countdownDuration, countdownText));

            yield return StartCoroutine(RoundTime());

            EnemyManager.Instance.DestroyAllEnemies();

            yield return new WaitForSeconds(1);

            PlayerManager.Instance.DisableAllPlayerMovement();
            yield return StartCoroutine(Countdown(5f, countdownText));

            yield return StartCoroutine(chestQTE.PlayQTE());


            //adjust enemy stats
            // ^^^ handled by Enemy Manager already

            //let players decide when to start the next round

            round++;
        }

        //losing condition, exit loop when a player's HP reaches 0
    }

    private bool IsGameRunning()
    {
        
        return true;
    }

    //list
   /* disable player movement on countdowns
    * dash cooldown ui
    * input ui on QTEs
    * fix player input on QTEs
    * item choices / randomize gear
    * finalize player gear system
    * adjust difficulty (increase enemy stats) 
    * winning/losing conditions
    */
}
