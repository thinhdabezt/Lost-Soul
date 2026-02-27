using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public GameObject pauseMenuPanel;
    private bool isPaused = false;
    private Firebase firebaseManager;
    private PlayerController playerController;
    private ScoreManager scoreManager;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0)
        {
            firebaseManager = FindAnyObjectByType<Firebase>();
            playerController = PlayerController.Instance;
            scoreManager = ScoreManager.Instance;
        }

        if (scene.buildIndex == 0)
        {
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            isPaused = false;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuPanel == null) return;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel == null) return;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void SaveAndQuitToMenu()
    {
        Time.timeScale = 1f;
        if (firebaseManager == null || playerController == null || scoreManager == null)
        {
            SceneManager.LoadScene(0);
            return;
        }

        string username = PlayerPrefs.GetString("Username", "");
        int currentHealth = 100;
        int currentScore = 0;
        int currentLevel = playerController.CurrentLevel;

        firebaseManager.SavePlayerData(username, currentHealth, currentLevel, currentScore, () =>
        {
            // Destroy các singleton/persistent object
            DestroyDontDestroySingletons();

            SceneManager.LoadScene(0);
        });
    }

    public void ResetSave()
    {
        Time.timeScale = 1f;
        if (firebaseManager == null || playerController == null || scoreManager == null)
        {
            SceneManager.LoadScene(0);
            return;
        }

        string username = PlayerPrefs.GetString("Username", "");
        int currentHealth = 100;
        int currentScore = 0;
        int currentLevel = 1;

        firebaseManager.SavePlayerData(username, currentHealth, currentLevel, currentScore, () =>
        {
            // Destroy các singleton/persistent object
            DestroyDontDestroySingletons();

            SceneManager.LoadScene(0);
        });
    }

    public void DestroyDontDestroySingletons()
    {
        if (PlayerController.Instance != null)
            Destroy(PlayerController.Instance.gameObject);
        if (ScoreManager.Instance != null)
            Destroy(ScoreManager.Instance.gameObject);
        if (PauseManager.Instance != null)
            Destroy(PauseManager.Instance.gameObject);
        if (UIManager.Instance != null)
            Destroy(UIManager.Instance.gameObject);
        if (SceneFader.Instance != null)
            Destroy(SceneFader.Instance.gameObject);
        if (GameOver.Instance != null)
            Destroy(GameOver.Instance.gameObject);

        foreach (var firebase in FindObjectsByType<Firebase>(FindObjectsSortMode.None))
        {
            if (firebase.gameObject.scene.name == "DontDestroyOnLoad")
                Destroy(firebase.gameObject);
        }
        foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (canvas.gameObject.scene.name == "DontDestroyOnLoad")
                Destroy(canvas.gameObject);
        }
    }
}