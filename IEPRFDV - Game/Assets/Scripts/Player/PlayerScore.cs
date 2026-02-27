using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScore : MonoBehaviour
{
    [Header("PlayerScore")]
    [SerializeField] private int score;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;

    public int Score
    {
        get => score;
        set => score = value;
    }

    private void Start()
    {
        score = 0;
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
}
