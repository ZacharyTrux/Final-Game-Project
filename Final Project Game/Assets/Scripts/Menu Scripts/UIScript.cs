using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour{
    private string titleScreen = "Title Screen";
    public GameObject pauseMenu;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public static UIScript Instance {get; private set;}
    
    private void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateHealthUI(){
        healthText.text = "HEALTH: " + PlayerManager.Instance.currHealth;
    }

    public void UpdateScoreUI(){
        //scoreText.text = "SCORE: " + ScoreManager.Instance.score;
    }


    private void Start(){
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void PauseGame(){
        Cursor.visible = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true); 
    }

    public void ContinueGame(){
        Cursor.visible = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void ForceRespawn(){
        PlayerManager.Instance.Respawn();
    }

    public void ReturnToMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleScreen);
        Destroy(gameObject);
    }

    
}
