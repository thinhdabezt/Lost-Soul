using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public GameObject pauseMenuPanel;
    private bool isPaused = false;

    // --- Các tham chiếu đến hệ thống khác ---
    private Firebase firebaseManager;
    private PlayerController playerController;
    private ScoreManager scoreManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Đăng ký sự kiện để tìm các manager khi scene thay đổi
    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void Update()
    {
        // Chỉ cho phép tạm dừng trong các màn chơi
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // Tìm các manager mỗi khi vào màn chơi mới
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0) // Nếu không phải Main Menu
        {
            firebaseManager = FindAnyObjectByType<Firebase>();
            playerController = PlayerController.Instance;
            scoreManager = ScoreManager.Instance;
        }

        // Tự động ẩn pause menu khi về màn hình chính
        if (scene.buildIndex == 0)
        {
            pauseMenuPanel.SetActive(false);
            isPaused = false;
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // HÀM QUAN TRỌNG: LƯU VÀ THOÁT
    public void SaveAndQuitToMenu()
    {
        //gameObject.SetActive(false);
        ScoreManager.Instance.level4Text.enabled = false;
        ScoreManager.Instance.scoreText.enabled = false;
        ScoreManager.Instance.maxScoreText.enabled = false;
        HealthBar.Instance.healthBarText.enabled = false;
        HealthBar.Instance.healthSlider.gameObject.SetActive(false);

        Time.timeScale = 1f; // Chạy lại thời gian

        if (firebaseManager == null || playerController == null || scoreManager == null)
        {
            Debug.LogError("Không tìm thấy đủ Manager để lưu game! Vẫn thoát về Menu.");
            SceneManager.LoadScene(0); // Vẫn cho về menu dù lỗi
            return;
        }

        // Lấy tất cả dữ liệu hiện tại
        string username = PlayerPrefs.GetString("Username", "");
        int currentHealth = playerController.GetComponent<Damageable>().Health;
        int currentScore = scoreManager.score;
        int currentLevel = playerController.CurrentLevel;

        // Gọi Firebase để lưu dữ liệu, sau khi lưu xong thì mới về Menu
        Debug.Log($"Đang lưu dữ liệu cho {username}: Máu={currentHealth}, Level={currentLevel}, Điểm={currentScore}");
        firebaseManager.SavePlayerData(username, currentHealth, currentLevel, currentScore, () =>
        {
            Debug.Log("Lưu thành công! Đang quay về Menu...");
            SceneManager.LoadScene(0);
        });
    }
}