using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel; 
    public GameObject usernamePanel;

    [Header("Username Elements")]
    public TMP_InputField usernameInputField; 
    public Button confirmStartButton; 

    void Start()
    {
        mainMenuPanel.SetActive(true);
        usernamePanel.SetActive(false);
    }

    void Update()
    {
        if (usernamePanel.activeSelf)
        {
            if (string.IsNullOrWhiteSpace(usernameInputField.text))
            {
                confirmStartButton.interactable = false;
            }
            else
            {
                confirmStartButton.interactable = true;
            }
        }
    }
    public void ShowUsernamePanel()
    {
        mainMenuPanel.SetActive(false);
        usernamePanel.SetActive(true);
    }

    public void ConfirmStartGame()
    {
        PlayerPrefs.SetString("Username", usernameInputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToMainMenu()
    {
        usernamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenHowToPlayScene()
    {
        SceneManager.LoadScene(5);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}