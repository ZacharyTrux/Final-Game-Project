using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenn : MonoBehaviour
{
    [SerializeField] private string firstLevelName = "Forest Level";

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}