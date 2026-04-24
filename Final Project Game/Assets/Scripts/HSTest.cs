using UnityEngine;
using TMPro;
using HighScore;

public class HSTest : MonoBehaviour{
    public TMP_InputField inputGameName;
    public TMP_InputField inputPlayerName;
    public TMP_InputField inputScore;

    public void InitGame(){
        HS.Init(this, "Siblings Curse");
    }

    public void SubmitScore(){
        HS.SubmitHighScore(this, inputPlayerName.text, int.Parse(inputScore.text));
    }

    public void ClearScores(){
        HS.Clear(this);
    }
}
