using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [SerializeField] private int round;
    [SerializeField] int roundDuration;
    [SerializeField] private float countdownDuration = 3;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI roundDurationText;

    //private float countdown;

    private bool roundEnded = false;

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
        StartCoroutine(Countdown());
        
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
        ExecuteRound();
        countdownText.text = "";
        //ExecuteRound();
    }

    IEnumerator RoundTime()
    {
        int timer = roundDuration;
        int secondsCount = timer;
        int minutesCount = timer / 60;
        //  if(roundDuration > 60) 

        string minutes = "";
        string seconds = "";

        while(timer > 0)
        {
            secondsCount = timer - minutesCount * 60;
            minutes = minutesCount.ToString();
            seconds = secondsCount.ToString();

            if (secondsCount % 60 == 0) seconds = "00";
            else if(secondsCount < 10) seconds = "0" + secondsCount.ToString();

            roundDurationText.text = minutes + ":" + seconds;

            if (secondsCount % 60 == 0) minutesCount -= 1;

            Debug.Log(timer);
            yield return new WaitForSeconds(1f);
            timer -= 1;
            
        }

        roundDurationText.text = "";
        roundEnded = true;
    }

    public void ExecuteRound()
    {
        StartCoroutine(RoundTime());

        //spawn enemies

        //list all enemies in an EnemyManager or smth

        //players defeat enemies

        //when timer runs out
        //clear all enemies

        //Chest QTE

        //When both players are ready

        //adjust enemies stats / modif diff
    
        //repeat loop


    }

}
