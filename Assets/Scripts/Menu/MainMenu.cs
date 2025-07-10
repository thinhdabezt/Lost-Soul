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

    firebaseManager.LoadPlayerData(username, (loadedData) =>
    {
        // Trường hợp 1: NGƯỜI CHƠI MỚI
        if (loadedData == null)
        {
            // Tạo dữ liệu mặc định và vào Level 1
            firebaseManager.SavePlayerData(username, 100, 1, 0, () =>
            {
                // Sau khi tạo user mới thành công, vào Level 1
                SceneManager.LoadScene("Level1");
            });
        }
        // Trường hợp 2: NGƯỜI CHƠI CŨ
        else
        {
            Debug.Log($"Người chơi cũ tìm thấy! Đang tải Level {loadedData.level}...");
            
            // Lấy level đã lưu và tạo tên scene tương ứng
            string sceneToLoad = "Level" + loadedData.level;

            // Tải scene tương ứng với level đã lưu
            SceneManager.LoadScene(sceneToLoad);
        }
    });
        if (ScoreManager.Instance != null)
        {
            if (!ScoreManager.Instance.scoreText.enabled)
            {
                ScoreManager.Instance.scoreText.enabled = true;
            }
        }
        if (HealthBar.Instance != null)
        {
            if (!HealthBar.Instance.healthBarText.enabled && !HealthBar.Instance.healthSlider.IsActive())
            {
                HealthBar.Instance.healthBarText.enabled = true;
                HealthBar.Instance.healthSlider.gameObject.SetActive(true);
            }
        }
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