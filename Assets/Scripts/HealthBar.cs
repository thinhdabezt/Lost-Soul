using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public TMP_Text healthBarText;
    public Slider healthSlider;

    Damageable playerDamageable;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerDamageable = player.GetComponent<Damageable>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthSlider.value = playerDamageable.Health / (float)playerDamageable.MaxHealth;
        healthBarText.text = "HP: " + playerDamageable.Health + "/" + playerDamageable.MaxHealth;
    }

    void OnEnable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    void OnDisable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPlayerHealthChanged(int health, int maxHealth)
    {
        healthSlider.value = health / (float)maxHealth;
        healthBarText.text = "HP: " + health + "/" + maxHealth;
    }
}
