using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(ScoreManager.Instance.score < ScoreManager.Instance.maxScore)
            {
                UIManager.Instance.ShowPopupMessage(collision.gameObject, "Collect all coin to proceed!!!");
            }
            else if (ScoreManager.Instance.score >= ScoreManager.Instance.maxScore)
            {
                SceneFader.Instance.FadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
