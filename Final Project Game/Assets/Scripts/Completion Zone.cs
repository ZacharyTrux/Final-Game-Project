using UnityEngine;

public class CompletionZone : MonoBehaviour{
    public void OnTriggerEnter(Collider other){
        print("trigger activated");
        if(other.GetComponent<PlayerTopDown>() != null){
            print("topdown player found");
            LevelManager.Instance.CompleteCurrSublevel();
        }
    }
}