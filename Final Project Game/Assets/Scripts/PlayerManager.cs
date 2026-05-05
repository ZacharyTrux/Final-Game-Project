using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour{
    public static PlayerManager Instance {get; private set;} 
    private PlayerInput controls;

    [Header("Players")]
    public Player2D player2D;
    public PlayerTopDown playerTopDown;

    [Header("Camera")]
    public CinemachineCamera oCam;
    public CinemachineCamera pCam;
    public CinemachineCamera tCam;

    public float tDuration = 0.8f;
    private bool is2DActive = true;
    private bool isTransitioning = false;
    public int currHealth;
    private int maxHealth = 3;
    private string loseScreen = "Lose Screen";

    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start(){
        controls = new PlayerInput();
        controls.Enable();
        currHealth = maxHealth;
        UIScript.Instance.UpdateHealthUI();
        Setup2D();
    }

    void Update(){
        if(controls.Player.Pause.WasPressedThisFrame()){
            print("game paused");
            UIScript.Instance.PauseGame();
        }
        if(controls.Player.Transition.WasPressedThisFrame() && !isTransitioning){
            StartCoroutine(ChangePerspective());
        }
    }

    IEnumerator ChangePerspective(){
        isTransitioning = true;
        if(is2DActive){
            SetupTopDown();
            is2DActive = false;
        }
        else{
            Setup2D();
            is2DActive = true;
        }
        yield return null;
        isTransitioning = false;
    }

    private void Setup2D(){
        oCam.Priority = 11;
        pCam.Priority = 9;
        player2D.enabled = true;
        playerTopDown.enabled = false;
    }

    private void SetupTopDown(){
        pCam.Priority = 11;
        oCam.Priority = 9;
        player2D.enabled = false;
        playerTopDown.enabled = true;
    }    

    public void checkHealth(){
        if(currHealth <= 0){
            Destroy(UIScript.Instance.gameObject);
            SceneManager.LoadScene(loseScreen);
        }
        UIScript.Instance.UpdateHealthUI();
    }

    public void Respawn(){
        if(is2DActive){
            player2D.transform.position = player2D.spawnPoint.position;
        }
        else{
            playerTopDown.transform.position = playerTopDown.spawnPoint.position;
        }
    }

    public void GroupRespawn(){
        player2D.transform.position = player2D.spawnPoint.position;
        playerTopDown.transform.position = playerTopDown.spawnPoint.position;
    }

    public void TakeDamage(){
        currHealth -= 1;
        Respawn();
        checkHealth();
    }
}