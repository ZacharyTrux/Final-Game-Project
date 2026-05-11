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
        if(ScoringManager.Instance.GetPlayerName() != null){ // player found dont begin on scoring menu
            ScoringUI.SetActive(false);
            MainMenuUI.SetActive(true);
        }
        else{ // player name needed
            ScoringUI.SetActive(true);
            MainMenuUI.SetActive(false);
        }
        InfoUI.SetActive(false);
        SettingsUI.SetActive(false);
    }

    public void StartGame(){
        SoundManager.Play(SoundType.BUTTON_CLICK);
        SceneManager.LoadScene("Forest Level");
        Cursor.visible = false;
    }

    public void ChangeSettingsMenu(){
        SoundManager.Play(SoundType.BUTTON_CLICK);
        MainMenuUI.SetActive(false);
        SettingsUI.SetActive(true);
    }

    public void QuitGame(){
        SoundManager.Play(SoundType.BUTTON_CLICK);
        Application.Quit();
    }

    public void UpdatePlayerName(){
        string playerName = nameInputField.text.Trim();
        if(playerName.Length > 0){
            ScoringManager.Instance.SetPlayerName(playerName);
            errorText.text = "";
            ScoringUI.SetActive(false);
            MainMenuUI.SetActive(true);
            SoundManager.Play(SoundType.BUTTON_CLICK);
        }
        else{
            errorText.text = "Please enter a valid name.";
        }
    }

    public void ChangeGameInfo(){
        SoundManager.Play(SoundType.BUTTON_CLICK);
        SettingsUI.SetActive(false);
        InfoUI.SetActive(true);
    }

    public void ChangeScoringMenu(){
        SoundManager.Play(SoundType.BUTTON_CLICK);
        ScoringUI.SetActive(true);
        SettingsUI.SetActive(false);
    }

    public void ReturnSettings(){
        SoundManager.Play(SoundType.BUTTON_CLICK);
        InfoUI.SetActive(false);
        SettingsUI.SetActive(true);
    }

    public void ReturnMainMenu(){
        SoundManager.Play(SoundType.BUTTON_CLICK);
        SettingsUI.SetActive(false);
        MainMenuUI.SetActive(true);
    }
}