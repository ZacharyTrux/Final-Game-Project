using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour{
    public void Start(){
        Cursor.visible = true;
    }

    public void StartGame(){
        SceneManager.LoadScene("Forest Level");
        Cursor.visible = false;
    }

    public void QuitGame(){
        Application.Quit();
    }
}