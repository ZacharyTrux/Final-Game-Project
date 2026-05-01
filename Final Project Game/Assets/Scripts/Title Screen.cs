using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen{
    public Scene firstLevel;

    public void StartGame(){
        SceneManager.SetActiveScene(firstLevel);
    }

    public void QuitGame(){
        Application.Quit();
    }
}