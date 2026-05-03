using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour{
    public Scene firstLevel;

    public void Start(){
        Cursor.visible = true;
    }

    public void StartGame(){
        SceneManager.SetActiveScene(firstLevel);
        Cursor.visible = false;
    }

    public void QuitGame(){
        Application.Quit();
    }
}