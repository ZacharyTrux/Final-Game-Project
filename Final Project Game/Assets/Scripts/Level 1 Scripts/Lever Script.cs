using UnityEngine;

public class LeverScript : MonoBehaviour, IInteractable{
    public bool isMultiUse = false;

    public MonoBehaviour[] linkedActions;
    private bool isUsed;
    private Animator leverAnimator;
    private bool playerInside = false;

    private void Awake(){
        isUsed = false;
        leverAnimator = GetComponent<Animator>();
    }


    public void Interact(){
        if(!isMultiUse && isUsed) return;
        isUsed = true;
        leverAnimator.SetTrigger("On");
        
        foreach(var action in linkedActions){
            print("action executed");
            var executable = action.GetComponent<IExecute>();
            executable.Execute();
        }
    }

    public void OnTriggerEnter(Collider other){
        if(other.GetComponent<Player2D>() != null){
            UIScript.Instance.ShowInteractPrompt();
            playerInside = true;
        }
    }

    public void OnTriggerExit(Collider other){
        if(playerInside){
            UIScript.Instance.ResetHintText();
        }
    }
}
