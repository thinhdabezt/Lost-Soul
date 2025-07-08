using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public GameObject pauseMenuPanel; // Kéo Panel Pause Menu vào đây

    private bool isPaused = false;

    private void Awake()
    {
        // Thiết lập Singleton và DontDestroyOnLoad để nó tồn tại qua các scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có một PauseManager khác, hủy đối tượng này đi
            Destroy(gameObject);
        }
    }

    void Update()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex >= 1 && currentSceneIndex <= 4)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Dừng thời gian trong game
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Chạy lại thời gian
        isPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Đang thoát về Menu...");
        // Luôn đảm bảo thời gian chạy lại bình thường trước khi chuyển scene
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        SceneManager.LoadScene(0); // 0 là buildIndex của Main Menu
    }
}