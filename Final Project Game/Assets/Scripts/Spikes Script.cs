using UnityEngine;

public class SpikesScript : MonoBehaviour{
    private void OnCollisionEnter(Collision collision){
        var player = collision.gameObject.GetComponent<Player2D>();
        player.HandleDrowning();
    }
}
