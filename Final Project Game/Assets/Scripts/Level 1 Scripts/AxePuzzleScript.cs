using UnityEngine;

public class AxePuzzleScript : MonoBehaviour{
    public void OnTriggerEnter(Collider collider){
        PlayerTopDown player = collider.GetComponent<PlayerTopDown>();
        if(player == null){
            return; 
        }
        GameObject item = player.GetHeldItem();
        if(item.CompareTag("Trigger")){
            Destroy(gameObject);
            player.Drop();
            Destroy(item);
        }
    }
}