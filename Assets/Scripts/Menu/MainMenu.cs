using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject usernamePanel;

    [Header("Username Elements")]
    public TMP_InputField usernameInputField;
    public Button confirmStartButton;

    [Header("Prefabs To Instantiate")]
    public GameObject playerPrefab;
    public GameObject uiManagerPrefab;
    public GameObject scoreManagerPrefab;
    public GameObject canvas;
    public GameObject pauseManagerPrefab;
    public GameObject firebaseManagerPrefab;

    //private static bool gameSystemsInitialized = false;

    // Hàm này được gọi bởi nút "Play"
    public void ShowUsernamePanel()
    {
        mainMenuPanel.SetActive(false);
        usernamePanel.SetActive(true);
    }

    // Hàm này được gọi bởi nút "Start"
    public void ConfirmStartGame()
    {
        confirmStartButton.interactable = false;
        string username = usernameInputField.text;
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.Save();

        StartCoroutine(InitializeSystemsAndLoadGame(username));
    }

    private IEnumerator InitializeSystemsAndLoadGame(string username)
    {
            Debug.Log("Initializing persistent systems for the first time...");
            Instantiate(firebaseManagerPrefab);
            Instantiate(playerPrefab);
            Instantiate(canvas);
            Instantiate(scoreManagerPrefab);
        Instantiate(uiManagerPrefab);
            Instantiate(pauseManagerPrefab);
            //gameSystemsInitialized = true;

        // 2. Đợi một frame để các hàm Awake() của đối tượng mới tạo có thời gian chạy
        yield return null;

        // 3. Bây giờ mới bắt đầu tải dữ liệu người chơi
        Firebase firebase = FindAnyObjectByType<Firebase>();
        firebase.LoadPlayerData(username, (loadedData) =>
        {
            if (loadedData == null)
            {
                // Người chơi mới: Lưu dữ liệu mặc định rồi vào Level 1
                firebase.SavePlayerData(username, 100, 1, 0, () => {
                    PlayerController.IsLoadingFromMainMenu = true;
                    SceneManager.LoadScene("Level1");
                });
            }
            else
            {
                // Người chơi cũ: Vào thẳng level đã lưu
                string sceneToLoad = "Level" + loadedData.level;
                PlayerController.IsLoadingFromMainMenu = true;
                SceneManager.LoadScene(sceneToLoad);
            }
        });

    }
    // Các hàm khác không thay đổi
    public void BackToMainMenu() { usernamePanel.SetActive(false); mainMenuPanel.SetActive(true); }
    public void OpenHowToPlayScene() { SceneManager.LoadScene(5); }
    public void QuitGame() { Application.Quit(); }
}

// Đoạn này để ngoài class MainMenu, cuối file
#if UNITY_EDITOR
public static class MainMenuStaticReset
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetStaticsOnPlay()
    {
        // Reset lại biến static mỗi lần nhấn Play trong Editor
        var type = typeof(MainMenu);
        var field = type.GetField("gameSystemsInitialized", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (field != null)
            field.SetValue(null, false);
    }
}
#endif
