using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;

public class PlayerManager : MonoBehaviour{
    [Header("Players")]
    public Player2D player2D;
    public PlayerTopDown playerTopDown;

    [Header("Camera")]
    public CinemachineCamera oCam;
    public CinemachineCamera pCam;

    public float tDuration = 0.8f;
    private bool is2DActive = true;
    private bool isTransitioning = false;
    //public PlayerManager Instance = {get; private set;};

    void Start(){
        Setup2D();
    }

    void Update(){
        if(Keyboard.current.tabKey.wasPressedThisFrame && !isTransitioning){
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
        yield return new WaitForSeconds(tDuration);
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




    
}