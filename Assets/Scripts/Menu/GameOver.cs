using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    Damageable playerDamageable;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerDamageable = player.GetComponent<Damageable>();
    }

    private void OnEnable()
    {
        playerDamageable.OnPlayerDeath.AddListener(ShowDeathScreen);
    }

    private void OnDisable()
    {
        playerDamageable.OnPlayerDeath.RemoveListener(ShowDeathScreen);
    }

    public void ShowDeathScreen()
    {
        gameObject.SetActive(true);
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
