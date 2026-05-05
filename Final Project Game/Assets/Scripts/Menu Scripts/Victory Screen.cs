using UnityEngine;
using UnityEngine.SceneManagement;


public class VictoryScreen : MonoBehaviour{
    public string title = "Title Screen";
    public string levelOne = "Forest Level";

    private void Start(){
        Cursor.visible = true;
    }

    public void Restart(){
        SceneManager.LoadScene(levelOne);
    }

    public void MainMenu(){
        SceneManager.LoadScene(title);
    }
}
