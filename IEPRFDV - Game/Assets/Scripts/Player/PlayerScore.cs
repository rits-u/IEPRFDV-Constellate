using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScore : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private int score;
    [SerializeField] private int roundStreak;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;

    public int Score
    {
        get => score;
        set => score = value;
    }

    public int RoundStreak
    {
        get => roundStreak;
        set => roundStreak = value;
    }

    private void Start()
    {
        score = 0;
        roundStreak = 1;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void CalculateScore(int enemyPoints)
    {
        int round = RoundManager.Instance.RoundNumber;
        int calc = enemyPoints * round * roundStreak;
        score += calc;
        //Debug.Log($"{gameObject.name} gained {calc}. Total points: {score}");

        UpdateScoreUI();
    }

    //will be based on round * streak surviving
}
