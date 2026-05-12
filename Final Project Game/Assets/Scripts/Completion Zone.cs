using UnityEngine;
using TMPro;
using System.Collections;

public class CompletionZone : MonoBehaviour{
    public GameObject player2DTarget;
    public GameObject playerTDTarget;

    private bool present2D = false;
    private bool presentTD = false;
    private bool isCompleted = false;
    private TextMeshProUGUI hintText;

    private Transform player2DTransform;
    private Transform playerTDTransform;

    // animation variables
    private float alignDuration = 1.0f;
    private float alignElasped = 0f;
    
    private float timeElapsed = 0f;
    public float duration = 2f;
    private float spinSpeed = 1000f;



    private void Awake(){
        if(hintText == null){
            hintText = GameObject.FindWithTag("Hint").GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnTriggerEnter(Collider other){
        UIScript.Instance.ShowCompletionPrompt();
        if(other.GetComponent<PlayerTopDown>() != null){
            presentTD = true;
            playerTDTransform = other.transform;
        }
        if(other.GetComponent<Player2D>() != null){
            present2D = true;
            player2DTransform = other.transform;
        }
        if(present2D && presentTD && !isCompleted){
            hintText.text = "";
            isCompleted = true;
            StartCoroutine(PortalAnimation());
        }
    }

    private void OnTriggerExit(Collider other){
        UIScript.Instance.ResetHintText();
        if(other.GetComponent<Player2D>() != null){
            present2D = false;
        }
        if(other.GetComponent<PlayerTopDown>() != null){
            presentTD = false;
        }
    }
    
    private IEnumerator PortalAnimation(){
        PlayerManager.Instance.enabled = false; 
        // move players to portal spot 
        Vector3 start2DPos = player2DTransform.position;
        Vector3 startTDPos = playerTDTransform.position;
        while(alignElasped < alignDuration){
            alignElasped += Time.deltaTime;
            float progress = Mathf.Clamp01(alignElasped / alignDuration);

            // move players to their respective targets 
            player2DTransform.position = Vector3.Lerp(start2DPos, player2DTarget.transform.position, progress);
            playerTDTransform.position = Vector3.Lerp(startTDPos, playerTDTarget.transform.position, progress);

            yield return null;
        }

        player2DTransform.position = player2DTarget.transform.position;
        playerTDTransform.position = playerTDTarget.transform.position;
        yield return new WaitForSeconds(1f);

        // portal animation 
        start2DPos = player2DTransform.position;
        startTDPos = playerTDTransform.position;
        var start2DRot = player2DTransform.rotation;
        var startTDRot = playerTDTransform.rotation;
        Vector3 portalCenter = transform.position;

        Vector3 startScale2D = player2DTransform.localScale;
        Vector3 startScaleTD = playerTDTransform.localScale;
        SoundManager.Play(SoundType.PORTAL);

        while(timeElapsed < duration){
            timeElapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(timeElapsed / duration);
            float easeProgress = Mathf.Pow(progress, 3);

            player2DTransform.position = Vector3.Lerp(start2DPos, portalCenter, easeProgress);
            playerTDTransform.position = Vector3.Lerp(startTDPos, portalCenter, easeProgress);

            player2DTransform.localScale = Vector3.Lerp(startScale2D, Vector3.zero, easeProgress);
            playerTDTransform.localScale = Vector3.Lerp(startScaleTD, Vector3.zero, easeProgress);

            float currentSpin = Mathf.Lerp(0, spinSpeed, easeProgress);
            player2DTransform.Rotate(Vector3.forward, currentSpin * Time.deltaTime);
            playerTDTransform.Rotate(Vector3.up, currentSpin * Time.deltaTime);

            yield return null;
        }
        SoundManager.Stop(SoundType.PORTAL);

        // reset player transforms and complete level
        player2DTransform.localScale = startScale2D;
        playerTDTransform.localScale = startScaleTD;
        player2DTransform.rotation = start2DRot;
        playerTDTransform.rotation = startTDRot;
        PlayerManager.Instance.enabled = true; 
        LevelManager.Instance.CompleteCurrSublevel();
    }
}