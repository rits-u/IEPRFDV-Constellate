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

    [Header("Properties")]
    [SerializeField] private int round;
    [SerializeField] int roundDuration;
    [SerializeField] private float countdownDuration = 3f;

    [Header("UI Elements")]
    [SerializeField] private Countdown countdown;
    [SerializeField] private TextMeshProUGUI roundNumberText;
    [SerializeField] private TextMeshProUGUI roundDurationText;

    [Header("QTE")]
    [SerializeField] private float qteCountdown = 5f;
    [SerializeField] private QuickTimeEvent chestQTE;

    [Header("References")]
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private GameObject QTEPrefab;


    
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

    //IEnumerator Countdown()
    //{
    //    float countdown = countdownDuration;
       
    //    while (countdown > 0)
    //    {
    //        countdownText.text = countdown.ToString();
    //        yield return new WaitForSeconds(1f);
    //        countdown -= 1;
    //    }

    //  //  countdownText.text = "START!";
       
    //    countdownText.text = "";
    //    //ExecuteRound();
    //}

    //IEnumerator Countdown(float duration, TextMeshProUGUI textUI)
    //{
    //    float countdown = duration;

    //    while (countdown > 0)
    //    {
    //        textUI.text = Mathf.CeilToInt(countdown).ToString();
    //        yield return new WaitForSeconds(1f);
    //        countdown -= 1f;
    //    }

    //    textUI.text = "";
    //}

    IEnumerator RoundTime()
    {
        roundEnded = false;
        Debug.Log($"RM: Start round time");
        int timer = roundDuration;
        int secondsCount = timer;
        int minutesCount = timer / 60;
        //  if(roundDuration > 60) 

        string minutes = "";
        string seconds = "";

        Debug.Log($"RM: RT: Start Spawning");
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

        Debug.Log($"RM: RT: Stop Spawning");
        SpawnManager.Instance.StopSpawning();
        //EnemyManager.Instance.UnregisterAllEnemies();

        roundDurationText.text = "";
        roundEnded = true;
    }

    public void ExecuteRound()
    {
        Debug.Log($"RM: Start ExecuteRound");
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
        Debug.Log($"RM: Start RoundFlow");
        //have !gameOver condition
        while (IsGameRunning())
        {
            roundNumberText.text = "Round: " + round;

            Debug.Log($"RM: Start RoundFlow While loop");
            //yield return StartCoroutine(Countdown());
            PlayerManager.Instance.EnableAllPlayerMovement();
            yield return countdown.CountdownTo(countdownDuration);

            yield return StartCoroutine(RoundTime());

            EnemyManager.Instance.DestroyAllEnemies();

            yield return new WaitForSeconds(1);

            PlayerManager.Instance.DisableAllPlayerMovement();
         //   yield return countdown.CountdownTo(qteCountdown);
            // yield return StartCoroutine(Countdown(5f, countdownText));

            Debug.Log($"RM: WL: instance qte1 ");
            RunQTE(player1);
            RunQTE(player2);


            //adjust enemy stats
            // ^^^ handled by Enemy Manager already

            //let players decide when to start the next round

            round++;
        }
        Debug.Log($"RM: End RoundFlow While loop ");

        //losing condition, exit loop when a player's HP reaches 0
    }

    private bool IsGameRunning()
    {
        
        return true;
    }

    //(fix) make sure qte destroy itself after
    private IEnumerator RunQTE(GameObject player)
    {
        GameObject qte = Instantiate(QTEPrefab, UICanvas.transform, false);
        yield return StartCoroutine(qte.GetComponent<QuickTimeEvent>().PlayQTE(player));
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
