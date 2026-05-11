using UnityEngine;
using HighScore;
using TMPro;

public class ScoringManager : MonoBehaviour{
    private int currScore;
    private string playerName;
    private GameObject nameTextBox;

    private float currentTime;
    private bool isTimerRunning = false;

    public int maxTimeBonus = 1000;
    public int scorePentaltyPerSecond = 10;

    private int numSwaps = 0;
    public int maxSwapsForBonus = 250;
    public int swapPenalty = 25;

    public static ScoringManager Instance { get; private set; }

    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start(){
        currScore = 0;
        HS.Init(this, "Siblings Curse");
    }

    private void Update(){
        if(isTimerRunning){
            currentTime += Time.deltaTime;
        }
    }

    public void StartLevelTracking(){
        currentTime = 0f;
        numSwaps = 0;
        isTimerRunning = true;
    }
    public void StopLevelTracking(){
        isTimerRunning = false;
        ApplyBonuses();
    }

    public void ApplyBonuses(){
        int secondsTaken = Mathf.FloorToInt(currentTime);
        int timeBonus = Mathf.Max(0, maxTimeBonus - (secondsTaken * scorePentaltyPerSecond));
        int swapBonus = Mathf.Max(0, maxSwapsForBonus - (numSwaps * swapPenalty));
        int combinedBonus = timeBonus + swapBonus;
        AddScore(combinedBonus);
    }

    public void SubmitScore(){
        HS.SubmitHighScore(this, playerName, currScore);
    }

    public void UpdateScore(int score){
        currScore = score;
    }

    public void AddScore(int amount){
        currScore += amount;
        if(UIScript.Instance != null){
            UIScript.Instance.UpdateScoreUI();
        }
        Debug.Log("Score: " + currScore);
    }

    public void SetPlayerName(string name){
        this.playerName = name;
    }

    public string GetPlayerName(){
        return playerName;
    }

    public int GetCurrentScore(){
        return currScore;
    }

    public void IncreaseSwaps(){
        numSwaps += 1;
    }

    public void ResetScore(){
        numSwaps = 0;
        currScore = 0;
        currentTime = 0f;
    }
}