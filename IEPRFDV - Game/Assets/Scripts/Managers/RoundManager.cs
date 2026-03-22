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
 //   [SerializeField] private float countdownDuration = 3f;

    [Header("UI Elements")]
  //  [SerializeField] private Countdown countdown;
    [SerializeField] private TextMeshProUGUI roundNumberText;
    [SerializeField] private TextMeshProUGUI roundDurationText;
    [SerializeField] private GameObject startGamePrompt;
    [SerializeField] private GameObject startButton;

    [Header("Triggers")]
    [SerializeField] private RoundTriggerArea roundTrigger;
    [SerializeField] private RewardTriggerArea rewardTrigger;
    //[SerializeField] private float qteCountdown = 5f;
    //[SerializeField] private QuickTimeEvent chestQTE;

    [Header("References")]
    [SerializeField] private GameObject UICanvas;
    //[SerializeField] private GameObject player1;
    //[SerializeField] private GameObject player2;
    //[SerializeField] private GameObject QTEPrefab;

    [SerializeField] private KeyCode startKey;
    
    private UICanvas canvas;


    //private float countdown;
    private bool hasStarted = false;
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
        if (startKey == KeyCode.None)
        {
            startKey = KeyCode.Space;
        }
        canvas = UICanvas.GetComponent<UICanvas>();
    }

    private void Update()
    {
        //if (!hasStarted && Input.GetKeyDown(startKey))
        //{
        //    hasStarted = true;
        // //   CountdownToStart();
        //    startButton.SetActive(false);
        //    startGamePrompt.SetActive(false);
        //}
        if (canvas.GetIsPaused()) PauseGame();
        else ResumeGame();
    }
    private void Start()
    {
        //CountdownToStart();
    }


    //public void CountdownToStart()
    //{
    //    // countdownPanel.SetActive(true);
    //    //StartCoroutine(Countdown());
    //    //StartCoroutine(RoundFlow());

    //}
    IEnumerator RoundTime()
    {
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
        Debug.Log($"RM: Start ExecuteRound");
        StartCoroutine(RoundProper());
        DeactivateRoundTrigger();

    }

    private IEnumerator RoundProper()
    {
        //while(true) {
        roundNumberText.text = $"Round: {round}";

        PlayerManager.Instance.EnableAllPlayerMovement();
        yield return StartCoroutine(RoundTime());
        EnemyManager.Instance.DestroyAllEnemies();
        yield return new WaitForSeconds(1);

        //reward qte phase
        ActivateRewardTrigger();
        PlayerManager.Instance.IncrementRoundStreak();

        round++;
       // }
    }

    public void NextRound()
    {
        PlayerManager.Instance.EnableAllPlayerMovement();
        DeactivateRewardTrigger();
        StartCoroutine(PrepareTrigger());
        //ExecuteRound();
    }

    IEnumerator PrepareTrigger()
    {
        yield return new WaitForSeconds(2f);
        ActivateRoundTrigger();
    }

    private void ActivateRoundTrigger()
    {
        roundTrigger.gameObject.SetActive(true);
    }

    private void DeactivateRoundTrigger()
    {
        roundTrigger.gameObject.SetActive(false);
    }

    private void ActivateRewardTrigger()
    {
        rewardTrigger.gameObject.SetActive(true);
    }

    private void DeactivateRewardTrigger()
    {
        rewardTrigger.gameObject.SetActive(false);
    }

    private void CheckPlayersCondition()
    {
        
    }

    public void StopRound()
    {
        StartCoroutine(StopRoundRoutine());
    }

    private IEnumerator StopRoundRoutine()
    {
        yield return new WaitForSeconds(2f);

        StopAllCoroutines();
        SpawnManager.Instance.StopSpawning();
        EnemyManager.Instance.DestroyAllEnemies();
        UIManager.Instance.DisableGameCanvas();

      //  yield return new WaitForSeconds(1f);
        UIManager.Instance.OpenScreen("Results");

        Debug.Log("game end");
    }


    //private IEnumerator RoundFlow()
    //{
    //    Debug.Log($"RM: Start RoundFlow");
    //    //have !gameOver condition
    //    while (IsGameRunning())
    //    {
    //        roundNumberText.text = "Round: " + round;

    //        Debug.Log($"RM: Start RoundFlow While loop");
    //        //yield return StartCoroutine(Countdown());
    //        PlayerManager.Instance.EnableAllPlayerMovement();
    //        yield return countdown.CountdownTo(countdownDuration);

    //        yield return StartCoroutine(RoundTime());

    //        EnemyManager.Instance.DestroyAllEnemies();

    //        yield return new WaitForSeconds(1);

    //        PlayerManager.Instance.DisableAllPlayerMovement();
    //     //   yield return countdown.CountdownTo(qteCountdown);
    //        // yield return StartCoroutine(Countdown(5f, countdownText));

    //        Debug.Log($"RM: WL: instance qte1 ");
    //        RunQTE(player1);
    //        RunQTE(player2);


            //Debug.Log($"RM: WL: instance qte1 ");
            //RunQTE(player1);
            //RunQTE(player2);

    //        //let players decide when to start the next round

    //        round++;
    //    }
    //    Debug.Log($"RM: End RoundFlow While loop ");

    //    //losing condition, exit loop when a player's HP reaches 0
    //}

    private bool IsGameRunning()
    {
        
        return true;
    }
    private void PauseGame()
    {
        Time.timeScale = 0f;
    }
    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    //(fix) make sure qte destroy itself after
    //private IEnumerator RunQTE(GameObject player)
    //{
    //    GameObject qte = Instantiate(QTEPrefab, UICanvas.transform, false);
    //    yield return StartCoroutine(qte.GetComponent<QuickTimeEvent>().PlayQTE(player));
    //}


    //list
   /* disable player movement on countdowns
    * dash cooldown ui
    * finalize player gear system
    * adjust difficulty (increase enemy stats) 
    * winning/losing conditions
    */
}
