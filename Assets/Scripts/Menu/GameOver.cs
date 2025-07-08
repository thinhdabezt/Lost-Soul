using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    Damageable playerDamageable;
    Firebase firebaseManager;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerDamageable = player.GetComponent<Damageable>();
        }

        firebaseManager = FindAnyObjectByType<Firebase>();
    }

    private void OnEnable()
    {
        if (playerDamageable != null)
        {
            playerDamageable.OnPlayerDeath.AddListener(ShowDeathScreen);
        }
    }

    private void OnDisable()
    {
        if (playerDamageable != null)
        {
            playerDamageable.OnPlayerDeath.RemoveListener(ShowDeathScreen);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}