using UnityEngine;

public class CompletionZone : MonoBehaviour{
    private bool present2D = false;
    private bool presentTD = false;

    private void OnTriggerEnter(Collider other){
        if(other.GetComponent<PlayerTopDown>() != null){
            presentTD = true;
        }
        if(other.GetComponent<Player2D>() != null){
            present2D = true;
        }
        if(present2D && presentTD){
            LevelManager.Instance.CompleteCurrSublevel();
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.GetComponent<Player2D>() != null){
            present2D = false;
        }
        if(other.GetComponent<PlayerTopDown>() != null){
            presentTD = false;
        }
    }
}