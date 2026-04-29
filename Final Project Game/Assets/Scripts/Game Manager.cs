using UnityEngine;
using HighScore;

public class GameManager : MonoBehaviour{
    private int score;
    private float maxLevelTime;
    private float currTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        score = 0;
        HS.Init(this, "Siblings Curse");
    }

    // Update is called once per frame
    void Update(){
        
    }
}
