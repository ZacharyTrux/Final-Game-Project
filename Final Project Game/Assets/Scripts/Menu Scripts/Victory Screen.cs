using UnityEngine;
using UnityEngine.SceneManagement;


public class VictoryScreen : MonoBehaviour{
    public Scene title;
    public Scene levelOne;
    public void Restart(){
        SceneManager.SetActiveScene(levelOne);
    }

    public void MainMenu(){
        SceneManager.SetActiveScene(title);
    }
}
