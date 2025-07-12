using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject healthTextPrefab;
    public GameObject damageTextPrefab;
    public GameObject messageTextPrefab;

    public Canvas canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        canvas = FindAnyObjectByType<Canvas>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        CharacterEvents.characterDamaged += (TookDamage);
        CharacterEvents.characterHealed += (Healed);
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged -= (TookDamage);
        CharacterEvents.characterHealed -= (Healed);
    }

    public void TookDamage(GameObject character, int damageReceived)
    {
        //Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        //GameObject go = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
        //Canvas sceneCanvas = Object.FindFirstObjectByType<Canvas>();
        //if (sceneCanvas != null)
        //    go.transform.SetParent(sceneCanvas.transform, false);
        //TMP_Text tmpText = go.GetComponent<TMP_Text>();
        //tmpText.text = damageReceived.ToString();

        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, canvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    public void Healed(GameObject character, int healthRestored)
    {
        //Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        //GameObject go = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity);
        //Canvas sceneCanvas = Object.FindFirstObjectByType<Canvas>();
        //if (sceneCanvas != null)
        //    go.transform.SetParent(sceneCanvas.transform, false);
        //TMP_Text tmpText = go.GetComponent<TMP_Text>();
        //tmpText.text = healthRestored.ToString();

        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, canvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void ShowPopupMessage(GameObject character, string message)
    {
        //Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        //GameObject go = Instantiate(messageTextPrefab, spawnPosition, Quaternion.identity);
        //Canvas sceneCanvas = Object.FindFirstObjectByType<Canvas>();
        //if (sceneCanvas != null)
        //    go.transform.SetParent(sceneCanvas.transform, false);
        //TMP_Text tmpText = go.GetComponent<TMP_Text>();
        //tmpText.text = message;

        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("No Canvas found for popup message.");
                return;
            }
        }

        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, canvas.transform).GetComponent<TMP_Text>();

        tmpText.text = message.ToString();
    }
}
