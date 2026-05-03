using UnityEngine;

public class LeverScript : MonoBehaviour, IInteractable{
    public bool isMultiUse = false;

    public MonoBehaviour[] linkedActions;
    private bool isUsed;
    // private Animator leverAnimator;

    private void Awake(){
        isUsed = false;
    }


    public void Interact(){
        if(!isMultiUse && isUsed) return;
        
        isUsed = true;
        foreach(var action in linkedActions){
            print("action executed");
            var executable = action.GetComponent<IExecute>();
            executable.Execute();
        }
    }
}
