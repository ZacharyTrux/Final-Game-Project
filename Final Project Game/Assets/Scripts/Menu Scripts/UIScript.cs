using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIScript : MonoBehaviour{
    private string titleScreen = "Title Screen";
    public GameObject pauseMenu;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hintText;
    public static UIScript Instance {get; private set;}

    private PlayerInput controls;
    
    private void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start(){
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        controls = new PlayerInput();
        controls.Enable();
    }

    private void Update(){
        if(controls.Player.Pause.WasPressedThisFrame()){
            if(Time.timeScale == 0f){
                ContinueGame();
                PlayerManager.Instance.enabled = true;
            }
            else{
                PauseGame();
                PlayerManager.Instance.enabled = false;
            }
        }
    }

    public void UpdateHealthUI(){
        healthText.text = "HEALTH: " + PlayerManager.Instance.currHealth;
    }

    public void UpdateScoreUI(){
        scoreText.text = "SCORE: " + ScoringManager.Instance.GetCurrentScore().ToString("000000");
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

    public void ShowCompletionPrompt(){
        hintText.text = "Use [tab] to group up and complete the level!";
    }

    public void ShowInteractPrompt(){
        hintText.text = "Press [E] to interact";
    }

    public void ResetHintText(){
        hintText.text = "";
    }

    
}
