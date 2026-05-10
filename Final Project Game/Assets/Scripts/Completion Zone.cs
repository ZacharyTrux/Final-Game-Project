using UnityEngine;
using TMPro;

public class CompletionZone : MonoBehaviour{
    private bool present2D = false;
    private bool presentTD = false;
    private bool isCompleted = false;
    private TextMeshProUGUI hintText;

    private void Awake(){
        if(hintText == null){
            hintText = GameObject.FindWithTag("Hint").GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnTriggerEnter(Collider other){
        UIScript.Instance.ShowCompletionPrompt();
        if(other.GetComponent<PlayerTopDown>() != null){
            presentTD = true;
        }
        if(other.GetComponent<Player2D>() != null){
            present2D = true;
        }
        if(present2D && presentTD && !isCompleted){
            hintText.text = "";
            isCompleted = true;
            LevelManager.Instance.CompleteCurrSublevel();
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
}