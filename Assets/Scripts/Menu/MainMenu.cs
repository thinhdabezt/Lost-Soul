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

    [Header("External Managers")]
    public Firebase firebaseManager;
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
        string username = usernameInputField.text;
        confirmStartButton.interactable = false;
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.Save();

        firebaseManager.LoadPlayerData(username, (loadedData) => {
            if (loadedData == null)
            {
                firebaseManager.SavePlayerData(username, 100, 1, 0,() => {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                });
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        });
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