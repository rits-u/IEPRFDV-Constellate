using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class TriggerRewardArea : MonoBehaviour
{
    
    [SerializeField] private int playersDetected;
    [SerializeField] private float timeCheck;
    [SerializeField] private float timeSinceLast;
    [SerializeField] private bool startCount;

    [Header("UI Elements")]
    [SerializeField] private GameObject rewardPanel;

    private bool rewardDisplayed = false;
    private CircleCollider2D col;
    private ChestLoot chestLoot;
   

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
        chestLoot = GetComponent<ChestLoot>();
    }

    private void OnEnable()
    {
        playersDetected = 0;
        timeSinceLast = 0;
        startCount = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playersDetected++;
        }

        if(playersDetected == 2)
        {
         //   PlayerManager.Instance.DisableAllPlayerMovement();
            startCount = true;
        }
    }

    private void Update()
    {
        if (startCount) timeSinceLast += Time.deltaTime;

        if (timeSinceLast >= timeCheck && !rewardDisplayed)
        {
            chestLoot.RandomizeLoot();
            ShowRewards();
            PlayerManager.Instance.DisableAllPlayerMovement();
            timeSinceLast = 0;
            rewardDisplayed = true;
            startCount = false;
        }
    }

    //private void OnTriggerStay2D(Collider other)
    //{

    //}



    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timeSinceLast = 0;
            startCount = false;
            playersDetected--;
        }
    }

    private void ShowRewards()
    {
        rewardPanel.LeanMoveY(540, 1.5f);
     }
}
