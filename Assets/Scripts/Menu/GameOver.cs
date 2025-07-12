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
        if (firebaseManager == null || playerDamageable == null)
        {
            Debug.LogError("Không thể lưu dữ liệu: Thiếu FirebaseManager hoặc PlayerDamageable!");
            return;
        }

        // Lấy thông tin cần lưu
        string username = PlayerPrefs.GetString("Username", "");

        // LẤY CÁC GIÁ TRỊ HIỆN TẠI
        int currentHealth = playerDamageable.Health; // Tại thời điểm chết, giá trị này sẽ là 0
        int currentLevel = PlayerController.Instance.CurrentLevel; // Lấy level từ PlayerController
        int finalScore = ScoreManager.Instance.score;

        if (!string.IsNullOrEmpty(username))
        {
            Debug.Log($"Đang lưu dữ liệu cho {username}: Máu={currentHealth}, Level={currentLevel}, Điểm={finalScore}");
            firebaseManager.SavePlayerData(username, currentHealth, currentLevel, finalScore);
        }
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenMainMenu()
    {
        gameOverPanel.SetActive(false);
        ScoreManager.Instance.scoreText.enabled = false;
        HealthBar.Instance.healthBarText.enabled = false;
        HealthBar.Instance.healthSlider.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
}