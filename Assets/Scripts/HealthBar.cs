using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static HealthBar Instance;

    public TMP_Text healthBarText;
    public Slider healthSlider;

    GameObject player;
    Damageable playerDamageable;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerDamageable = player.GetComponent<Damageable>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetHealthBar();
    }

    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerDamageable = player.GetComponent<Damageable>();
        }

        if (playerDamageable != null && playerDamageable.IsAlive)
        {
            SetHealthBar();
        }
    }

    void OnEnable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    void OnDisable()
    {
        playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged); 
    }
    public void SetHealthBar()
    {
        healthSlider.value = playerDamageable.Health / (float)playerDamageable.MaxHealth;
        healthBarText.text = "HP: " + playerDamageable.Health + "/" + playerDamageable.MaxHealth;
    }
    public void OnPlayerHealthChanged(int health, int maxHealth)
    {
        healthSlider.value = health / (float)maxHealth;
        healthBarText.text = "HP: " + health + "/" + maxHealth;
    }
}
