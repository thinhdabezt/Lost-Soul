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
        canvas = Object.FindFirstObjectByType<Canvas>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, canvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    public void Healed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, canvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void ShowPopupMessage(GameObject character, string message)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(messageTextPrefab, spawnPosition, Quaternion.identity, canvas.transform).GetComponent<TMP_Text>();

        tmpText.text = message;
    }
}
