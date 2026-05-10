using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreen : MonoBehaviour{
    public GameObject ScoringUI;
    public GameObject MainMenuUI;
    public GameObject InfoUI;
    public GameObject SettingsUI;

    public TextMeshProUGUI errorText;
    public TMP_InputField nameInputField;


    public void Start(){
        Cursor.visible = true;
        ScoringUI.SetActive(true);
        MainMenuUI.SetActive(false);
        InfoUI.SetActive(false);
        SettingsUI.SetActive(false);
    }

    public void StartGame(){
        SceneManager.LoadScene("Forest Level");
        Cursor.visible = false;
    }

    public void ChangeSettingsMenu(){
        MainMenuUI.SetActive(false);
        SettingsUI.SetActive(true);
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void UpdatePlayerName(){
        string playerName = nameInputField.text.Trim();
        if(playerName.Length > 0){
            ScoringManager.Instance.SetPlayerName(playerName);
            errorText.text = "";
            ScoringUI.SetActive(false);
            MainMenuUI.SetActive(true);
        }
        else{
            errorText.text = "Please enter a valid name.";
        }
    }

    public void ChangeGameInfo(){
        SettingsUI.SetActive(false);
        InfoUI.SetActive(true);
    }

    public void ChangeScoringMenu(){
        ScoringUI.SetActive(true);
        SettingsUI.SetActive(false);
    }

    public void ReturnSettings(){
        InfoUI.SetActive(false);
        SettingsUI.SetActive(true);
    }

    public void ReturnMainMenu(){
        SettingsUI.SetActive(false);
        MainMenuUI.SetActive(true);
    }
}