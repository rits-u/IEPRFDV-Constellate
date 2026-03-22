using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ResultsUI : ScreenUI
{
    [Header("Panels")]
    [SerializeField] private ResultPanel p1Result;
    [SerializeField] private ResultPanel p2Result;

    [Header("UI Colors")]
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    [Header("Buttons")]
    [SerializeField] Button playAgainBtn;
    [SerializeField] Button quitBtn;

    [System.Serializable]
    public class ResultPanel
    {
        public Image resultBG;
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI scoreText;
    }

    public override void ShowScreenUI()
    {
        gameObject.SetActive(true);

        ShowResults();
        ShowScore();
        StartCoroutine(ShowButtons());
    }

    private void ShowResults()
    {
        Player winner = PlayerManager.Instance.DetermineWinner();

        if (winner == null)
        {
            ChangeUI(p1Result, "Tie");
            ChangeUI(p2Result, "Tie");
        }
        else
        {
            if(winner.ID == 1)
            {
                ChangeUI(p1Result, "Win");
                ChangeUI(p2Result, "Lose");
            }
            else if (winner.ID == 2)
            {
                ChangeUI(p1Result, "Lose");
                ChangeUI(p2Result, "Win");
            }
        }
    }

    private void ChangeUI(ResultPanel panel, string result)
    {
        switch(result)
        {
            case "Win":
                panel.resultBG.color = winColor;
                panel.resultText.text = "You Win!";
                break;
            case "Lose":
                panel.resultBG.color = loseColor;
                panel.resultText.text = "You Lose!";
                break;
            case "Tie":
                panel.resultBG.color = loseColor;
                panel.resultText.text = "Draw";
                break;
        }
    }

    private void ShowScore()
    {
        p1Result.scoreText.text = PlayerManager.Instance.GetPlayerScoreByID(1).ToString();
        p2Result.scoreText.text = PlayerManager.Instance.GetPlayerScoreByID(2).ToString();
    }


    public override void HideScreenUI() 
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        screenName = "Results";
    }

    private IEnumerator ShowButtons()
    {
        yield return new WaitForSeconds(1.5f);
        playAgainBtn.gameObject.SetActive(true);
        quitBtn.gameObject.SetActive(true);
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    public void OnPlayAgainButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
