using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;

    private GameObject player;
    Damageable playerDamageable;
    Firebase firebaseManager;
    public GameObject gameOverPanel;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        firebaseManager = FindAnyObjectByType<Firebase>();
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerDamageable = player.GetComponent<Damageable>();
        }

        if (playerDamageable != null && !playerDamageable.IsAlive && !gameOverPanel.activeSelf)
        {
            Debug.Log("Player is dead, showing death screen.");
            gameOverPanel.SetActive(true);
            SaveCurrentData();
        }
    }

    public void ShowDeathScreen()
    {
        gameObject.SetActive(true);
        SaveCurrentData();
    }

    private void SaveCurrentData()
    {
        if (firebaseManager == null)
        {
            Debug.LogError("Không thể lưu dữ liệu: Thiếu FirebaseManager!");
            return;
        }

        string username = PlayerPrefs.GetString("Username", "");
        int currentLevel = PlayerController.Instance.CurrentLevel;

        if (!string.IsNullOrEmpty(username))
        {
            Debug.Log($"Đang lưu dữ liệu cho {username}: Level={currentLevel}");
            // Lưu máu = 100, điểm = 0 (hoặc giữ nguyên nếu muốn)
            firebaseManager.SavePlayerData(username, 100, currentLevel, 0);
        }
    }


    public void RestartGame()
    {
        // Reset máu và điểm trước khi restart
        if (PlayerController.Instance != null)
        {
            var damageable = PlayerController.Instance.GetComponent<Damageable>();
            if (damageable != null)
                damageable.Initialize(100);
        }
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab);
        }
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }
        if (HealthBar.Instance != null)
        {
            HealthBar.Instance.SetHealthBar();
        }
        gameOverPanel.SetActive(false);
        // KHÔNG destroy singleton ở đây!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenMainMenu()
    {
        gameOverPanel.SetActive(false);

        // Reset máu và điểm về mặc định
        if (PlayerController.Instance != null)
        {
            var damageable = PlayerController.Instance.GetComponent<Damageable>();
            if (damageable != null)
                damageable.Initialize(100);
        }
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetScore(0);
        }

        // Ẩn UI
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.scoreText.enabled = false;
        if (HealthBar.Instance != null)
        {
            HealthBar.Instance.healthBarText.enabled = false;
            HealthBar.Instance.healthSlider.gameObject.SetActive(false);
        }

        // Destroy các singleton/persistent object
        DestroyDontDestroySingletons();

        SceneManager.LoadScene(0);
    }

    private void DestroyDontDestroySingletons()
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

        foreach (var firebase in Object.FindObjectsByType<Firebase>(FindObjectsSortMode.None))
        {
            if (firebase.gameObject.scene.name == "DontDestroyOnLoad")
                Destroy(firebase.gameObject);
        }
        foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (canvas.gameObject.scene.name == "DontDestroyOnLoad")
                Destroy(canvas.gameObject);
        }
    }

}