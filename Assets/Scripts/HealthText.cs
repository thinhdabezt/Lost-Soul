using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    public Vector3 moveSpeed = new Vector3(0, 75, 0);

    public float fadeTime = 1.0f;

    private float timeElapsed;

    private Color startColor;

    RectTransform textTransform;
    TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        textTransform = GetComponent<RectTransform>();
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        startColor = textMeshProUGUI.color;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        textTransform.position += moveSpeed * Time.deltaTime;
        timeElapsed += Time.deltaTime;

        if (timeElapsed < fadeTime)
        {
            float newAlpha = startColor.a * (1 - (timeElapsed / fadeTime));
            textMeshProUGUI.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
