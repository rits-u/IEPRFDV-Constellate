using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardTriggerArea : MonoBehaviour
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

    private Vector3 panelStartPosition;
   

    private void Start()
    {
        panelStartPosition = rewardPanel.GetComponent<RectTransform>().position;
        col = GetComponent<CircleCollider2D>();
        chestLoot = GetComponentInParent<ChestLoot>();
    }

    private void OnEnable()
    {
        playersDetected = 0;
        timeSinceLast = 0;
        startCount = false;
        rewardDisplayed = false;
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

    private void ResetRewardPanel()
    {
        RectTransform rect = rewardPanel.GetComponent<RectTransform>();
        rect.position = panelStartPosition;
    }

    private void ShowRewardsScreen()
    {
        ResetRewardPanel();
        rewardPanel.LeanMoveY(540, leanDuration);
        startBtn.gameObject.SetActive(true);
     }

    public void HideRewardsScreen()
    {
        PlayerManager.Instance.EnableAllPlayerMovement();
        rewardPanel.LeanMoveY(1800, leanDuration);
        //StartCoroutine(DisableRewardArea());
        DisableArea();
    }

    private void DisableArea()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator DisableRewardArea()
    {
        yield return new WaitForSeconds(leanDuration + 0.5f);
        gameObject.SetActive(false);    
    }
}
