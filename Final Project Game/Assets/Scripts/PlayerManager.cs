using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerManager : MonoBehaviour{
    [Header("Players")]
    public Player2D player2D;
    public PlayerTopDown playerTopDown;

    [Header("Camera")]
    public Camera mainCamera;
    // get camera positions
    public Transform cam2DTarget; 
    public Transform camTopDownTarget;
    public float transitionDuration = 0.8f;

    private bool is2DActive = true;
    private bool isTransitioning = false;
    //public PlayerManager Instance = {get; private set;};

    void Start(){
        // grab camera positions
        mainCamera.transform.position = cam2DTarget.position;
        mainCamera.transform.rotation = cam2DTarget.rotation;

        player2D.enabled = true;
        playerTopDown.enabled = false;
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame && !isTransitioning)
            StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        isTransitioning = true;

        // Disable both players during the transition
        player2D.enabled = false;
        playerTopDown.enabled = false;

        is2DActive = !is2DActive;
        Transform target = is2DActive ? cam2DTarget : camTopDownTarget;

        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            mainCamera.transform.position = Vector3.Lerp(startPos, target.position, t);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, target.rotation, t);

            yield return null;
        }

        
        mainCamera.transform.position = target.position;
        mainCamera.transform.rotation = target.rotation;

        player2D.enabled = is2DActive;
        playerTopDown.enabled = !is2DActive;

        isTransitioning = false;
    }
}