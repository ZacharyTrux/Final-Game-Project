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
        playerTopDown.Drop();
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


    public void Respawn()
    {
        Debug.Log($"[PM] Respawn called — is2DActive: {is2DActive}, moving to: {(is2DActive ? player2D.SpawnPoint?.position : playerTopDown.SpawnPoint?.position)}");
        if (is2DActive)
        {
            player2D.transform.position = player2D.SpawnPoint.position;
        }
        else
        {
            playerTopDown.transform.position = playerTopDown.SpawnPoint.position;
        }
    }


    public void GroupRespawn()
    {
        Debug.LogWarning($"[PM] GroupRespawn called!\n{new System.Diagnostics.StackTrace()}");

        Rigidbody rb2D = player2D.GetComponent<Rigidbody>();
        Rigidbody rbTD = playerTopDown.GetComponent<Rigidbody>();

        rb2D.position = player2D.SpawnPoint.position;
        rb2D.linearVelocity = Vector3.zero;
        rb2D.angularVelocity = Vector3.zero;

        rbTD.position = playerTopDown.SpawnPoint.position;
        rbTD.linearVelocity = Vector3.zero;
        rbTD.angularVelocity = Vector3.zero;

        player2D.transform.position = player2D.SpawnPoint.position;
        playerTopDown.transform.position = playerTopDown.SpawnPoint.position;

        Physics.SyncTransforms();

        Debug.Log($"[PM] GroupRespawn done — 2D now at: {player2D.transform.position}");
    }

    public void TakeDamage()
    {
        if (LevelManager.Instance.isTransitioning) return;
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

    private Vector3 lastKnownPos;

    

    void LateUpdate()
    {
        if (player2D == null) return;

        if (Vector3.Distance(player2D.transform.position, lastKnownPos) > 2f)
        {
            Debug.LogWarning($"[PM] player2D MOVED UNEXPECTEDLY — from: {lastKnownPos} to: {player2D.transform.position}\n{new System.Diagnostics.StackTrace()}");
        }
        lastKnownPos = player2D.transform.position;
    }
}