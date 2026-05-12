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
    private int maxHealth = 5;
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
        if(controls.Player.Transition.WasPressedThisFrame() && !isTransitioning){
            StartCoroutine(ChangePerspective());
            ScoringManager.Instance.IncreaseSwaps();
        }
    }

    public void OnDisable()
    {
        if (controls != null)
        {
            controls.Disable();
        }

        if (player2D != null)
        {
            player2D.enabled = false;
        }

        if (playerTopDown != null)
        {
            playerTopDown.enabled = false;
        }
    }

    public void OnEnable()
    {
        if (controls != null)
        {
            controls.Enable();
        }

        if (player2D == null || playerTopDown == null)
        {
            return;
        }

        if (is2DActive)
        {
            player2D.enabled = true;
            playerTopDown.enabled = false;
        }
        else
        {
            player2D.enabled = false;
            playerTopDown.enabled = true;
        }
    }

    IEnumerator ChangePerspective(){
        isTransitioning = true;
        if(is2DActive){
            SetupTopDown();
        }
        else{
            Setup2D();
        }
        yield return null;
        isTransitioning = false;
    }

    public void Setup2D(){
        is2DActive = true;
        if (oCam != null) oCam.Priority = 11;
        if (pCam != null) pCam.Priority = 9;

        if (player2D != null) player2D.enabled = true;
        if (playerTopDown != null) playerTopDown.enabled = false;
    }

    public void SetupTopDown(){
        is2DActive = false;
        if (pCam != null) pCam.Priority = 11;
        if (oCam != null) oCam.Priority = 9;

        if (player2D != null) player2D.enabled = false;
        if (playerTopDown != null) playerTopDown.enabled = true;
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

    public void GroupRespawn()
    {
        player2D.transform.position = player2D.spawnPoint.position;
        oCam.PreviousStateIsValid = false;
        playerTopDown.transform.position = playerTopDown.spawnPoint.position;
    }

    public void TakeDamage(){
        currHealth -= 1;
        Respawn();
        checkHealth();
    }

    public bool Is2DActive(){
        return is2DActive;
    }

    public bool IsTopDownActive(){
        return !is2DActive;
    }
}