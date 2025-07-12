using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    Damageable bossDamageable;

    private void Awake()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
        {
            bossDamageable = boss.GetComponent<Damageable>();
        }
    }

    private void OnEnable()
    {
        if (bossDamageable != null)
        {
            bossDamageable.OnBossDeath.AddListener(ShowVictoryScreen);
        }
    }

    private void OnDisable()
    {
        if (bossDamageable != null)
        {
            bossDamageable.OnBossDeath.RemoveListener(ShowVictoryScreen);
        }
    }

    public void ShowVictoryScreen()
    {
        PlayerController.Instance.GetComponent<PlayerInput>().enabled = false;
        gameObject.SetActive(true);
    }

    public void OpenMainMenu()
    {
        PauseManager.Instance.ResetSave();
        gameObject.SetActive(false);
        ScoreManager.Instance.level4Text.enabled = false;
        HealthBar.Instance.healthBarText.enabled = false;
        HealthBar.Instance.healthSlider.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
