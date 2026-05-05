using UnityEngine;
using UnityEngine.SceneManagement;


public class VictoryScreen : MonoBehaviour{
    private void Start(){
        Cursor.visible = true;
    }

    public void Restart(){
        SceneManager.LoadScene("Forest Level");
    }

    public void MainMenu(){
        SceneManager.LoadScene("Title Screen");
    }
}
