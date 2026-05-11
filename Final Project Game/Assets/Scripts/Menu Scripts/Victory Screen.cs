using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryScreen : MonoBehaviour{
    public TextMeshProUGUI finalScoreText;

    private void Start(){
        Cursor.visible = true;
        if(finalScoreText != null){
            finalScoreText.text = "FINAL SCORE: " + ScoringManager.Instance.GetCurrentScore().ToString("000000");
        }
    }

    public void Restart(){
        SceneManager.LoadScene("Forest Level");
    }

    public void MainMenu(){
        SceneManager.LoadScene("Title Screen");
    }
}
