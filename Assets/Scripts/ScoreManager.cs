using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    [SerializeField] public int maxScore;

    public TMP_Text scoreText;
    public TMP_Text maxScoreText;

    private Coroutine maxScorePopRoutine;
    [SerializeField] private float duration;
    [SerializeField] private Vector3 targetScale;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        if (score >= maxScore && !maxScoreText.enabled)
        {
            scoreText.enabled = false;
            maxScoreText.enabled = true;

            if (maxScorePopRoutine == null)
            {
                maxScorePopRoutine = StartCoroutine(LoopPopMaxScore());
            }
        }
        else if (score < maxScore && maxScoreText.enabled)
        {
            scoreText.enabled = true;
            maxScoreText.enabled = false;
            if (maxScorePopRoutine != null)
            {
                StopCoroutine(maxScorePopRoutine);
                maxScorePopRoutine = null;
            }
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
        StartCoroutine(PopScore());
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }
    public void SetScore(int initialScore)
    {
        score = initialScore;
        UpdateScoreText();
    }

    public IEnumerator PopScore()
    {
        RectTransform rt = scoreText.GetComponent<RectTransform>();
        Vector3 originalScale = rt.localScale;

        rt.localScale = originalScale * 1.2f;
        yield return new WaitForSeconds(0.1f);
        rt.localScale = originalScale;
    }

    private IEnumerator LoopPopMaxScore()
    {
        RectTransform rt = maxScoreText.GetComponent<RectTransform>();
        Vector3 originalScale = rt.localScale;

        while (true)
        {
            // Scale up
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float eased = Mathf.SmoothStep(0f, 1f, t / duration);
                rt.localScale = Vector3.Lerp(originalScale, targetScale, eased);
                yield return null;
            }

            // Scale down
            t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float eased = Mathf.SmoothStep(0f, 1f, t / duration);
                rt.localScale = Vector3.Lerp(targetScale, originalScale, eased);
                yield return null;
            }

            yield return new WaitForSeconds(0.75f); // Wait before repeating
        }
    }
}
