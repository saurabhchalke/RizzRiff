using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject CreditsPanel;
    public GameObject CreditsButtons;
    public GameObject MenuButtons;
    public GameObject SongsPanel;
    public GameObject SongsButtons;
    public GameObject LevelsButtons;
    public GameObject ResumePanel;



    void Start() {
        AudioManager.instance.PlayMusic("menu");
    }

    public void StartGame() => SCManager.instance.LoadScene("Game");

    public void QuitGame() => Application.Quit();

    public void ShowCredits()
    {
        CreditsPanel.SetActive(true);
        CreditsButtons.SetActive(true);
        MenuButtons.SetActive(false);
    }

    public void HideCredits()
    {
        CreditsPanel.SetActive(false);
        CreditsButtons.SetActive(false);
        MenuButtons.SetActive(true);
    }

    public void ShowSongs()
    {
        SongsPanel.SetActive(true);
        SongsButtons.SetActive(true);
        MenuButtons.SetActive(false);
    }

    public void HideSongs()
    {
        SongsPanel.SetActive(false);
        SongsButtons.SetActive(false);
        MenuButtons.SetActive(true);
    }

    public void ShowLevels()
    {
        LevelsButtons.SetActive(true);
        MenuButtons.SetActive(false);
    }

    public void HideLevels()
    {
        ResumePanel.SetActive(false);
        LevelsButtons.SetActive(false);
        MenuButtons.SetActive(true);
    }
    
    //public void ShowSettings() => SettingsPanel.SetActive(true);

    //public void HideSettings() => // SettingsPanel.SetActive(false);

    public void GoToMenu() => SCManager.instance.LoadScene("Menu");
}
