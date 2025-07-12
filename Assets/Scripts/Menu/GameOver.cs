using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;

    Damageable playerDamageable;
    Firebase firebaseManager;
    public GameObject gameOverPanel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerDamageable = player.GetComponent<Damageable>();
        }

        firebaseManager = FindAnyObjectByType<Firebase>();
    }

    private void Update()
    {
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
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetScore(0);
        }

        gameOverPanel.SetActive(false);
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
        ScoreManager.Instance.scoreText.enabled = false;
        HealthBar.Instance.healthBarText.enabled = false;
        HealthBar.Instance.healthSlider.gameObject.SetActive(false);

        SceneManager.LoadScene(0);
    }

}