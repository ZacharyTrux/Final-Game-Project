using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryScreen : MonoBehaviour{
    public TextMeshProUGUI finalScoreText;

    private void Start(){
        Cursor.visible = true;
        ScoringManager.Instance.SubmitScore();
        if(finalScoreText != null){
            finalScoreText.text = "FINAL SCORE: " + ScoringManager.Instance.GetCurrentScore().ToString("000000");
        }
        ScoringManager.Instance.ResetScore();
    }

    public void Restart(){
        SoundManager.Play(SoundType.BUTTON_CLICK);
        SceneManager.LoadScene("Forest Level");
    }

    public void MainMenu(){
        SoundManager.Play(SoundType.BUTTON_CLICK);
        SceneManager.LoadScene(0);
    }
}
