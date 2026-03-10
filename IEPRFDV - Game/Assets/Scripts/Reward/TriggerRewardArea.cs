using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TriggerRewardArea : MonoBehaviour
{
    
    [SerializeField] private int playersDetected;
    [SerializeField] private float timeCheck;
    [SerializeField] private float timeSinceLast;
    [SerializeField] private bool startCount;
    [SerializeField] private float leanDuration = 1.5f;

    [Header("UI Elements")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Button startBtn;

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
            ShowRewardsScreen();
            PlayerManager.Instance.DisableAllPlayerMovement();
            timeSinceLast = 0;
            rewardDisplayed = true;
            startCount = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timeSinceLast = 0;
            startCount = false;
            playersDetected--;
        }
    }

    private void ShowRewardsScreen()
    {
        rewardPanel.LeanMoveY(540, leanDuration);
        startBtn.gameObject.SetActive(true);
     }

    public void HideRewardsScreen()
    {
        rewardPanel.LeanMoveY(-549, leanDuration);
        StartCoroutine(DisableRewardArea());
    }

    private IEnumerator DisableRewardArea()
    {
        yield return new WaitForSeconds(leanDuration + 0.5f);
        PlayerManager.Instance.EnableAllPlayerMovement();
        gameObject.SetActive(false);
    }
}
