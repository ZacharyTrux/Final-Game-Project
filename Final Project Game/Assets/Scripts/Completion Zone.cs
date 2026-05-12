using UnityEngine;
using TMPro;
using System.Collections;

public class CompletionZone : MonoBehaviour{
    private bool present2D = false;
    private bool presentTD = false;
    private bool isCompleted = false;
    private TextMeshProUGUI hintText;

    private Transform player2DTransform;
    private Transform playerTDTransform;
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
            //StartCoroutine(PortalAnimation());
            LevelManager.Instance.CompleteCurrSublevel();
        }
    }
    /*
    private IEnumerator PortalAnimation(){
        PlayerManager.Instance.DisableInputs(); 
        Vector3 start2DPos = player2DTransform.position;
        Vector3 startTDPos = playerTDTransform.position;
        Vector3 portalCenter = transform.position;

        Vector3 startScale2D = player2DTransform.localScale;
        Vector3 startScaleTD = playerTDTransform.localScale;

        

        //while(timeElapsed < duration){
            
        //}
        yield return null;
    }

    */

    private void OnTriggerExit(Collider other){
        UIScript.Instance.ResetHintText();
        if(other.GetComponent<Player2D>() != null){
            present2D = false;
        }
        if(other.GetComponent<PlayerTopDown>() != null){
            presentTD = false;
        }
    }
}