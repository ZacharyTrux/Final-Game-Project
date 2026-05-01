using UnityEngine;
using HighScore;
using TMPro;

public class ScoringSystem : MonoBehaviour{
    private int currScore;
    private string playerName;
    private GameObject nameTextBox;

    public static ScoringSystem Instance { get; private set; } 

    private void Awake(){
        if(Instance == null){
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

    public void SubmitScore(){
        HS.SubmitHighScore(this, playerName, currScore);
    }

    public void UpdateScore(int score){
        currScore = score;
    }

    public void SetPlayerName(){
        
    }
}