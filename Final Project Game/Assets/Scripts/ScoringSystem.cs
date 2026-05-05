using UnityEngine;
using HighScore;
using TMPro;

public class ScoringSystem : MonoBehaviour
{
    private int currScore;
    private string playerName;
    private GameObject nameTextBox;

    public TextMeshProUGUI scoreText;

    public static ScoringSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currScore = 0;
        HS.Init(this, "Siblings Curse");
        UpdateScoreUI();
    }

    public void SubmitScore()
    {
        HS.SubmitHighScore(this, playerName, currScore);
    }

    public void UpdateScore(int score)
    {
        currScore = score;
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currScore += amount;
        UpdateScoreUI();
        Debug.Log("Score: " + currScore);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + currScore.ToString("000000");
        }
        else
        {
            Debug.LogWarning("Score Text is not assigned in ScoringSystem.");
        }
    }

    public void SetPlayerName()
    {

    }
}