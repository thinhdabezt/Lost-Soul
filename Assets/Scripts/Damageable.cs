using Assets.Scripts;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> hit;
    public UnityEvent<int, int> healthChanged;
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnBossDeath;

    public enum DamageableType { Player, Enemy, Boss }
    public DamageableType damageableType;

    Animator animator;

    private bool isInvincible = false;
    private float timeSinceHit = 0;

    [SerializeField] private float invincibleTime = 0.25f;
    [SerializeField] private int _maxHealth = 100; 
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = Mathf.Max(0, value); }
    }

    [SerializeField] private int _health;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            healthChanged?.Invoke(_health, MaxHealth);

            if (_health <= 0)
            {
                if (damageableType == DamageableType.Player)
                {
                    OnPlayerDeath?.Invoke();
                    CharacterEvents.playerDeath?.Invoke();
                }
                else if (damageableType == DamageableType.Boss)
                {
                    OnBossDeath?.Invoke();
                }
                IsAlive = false;
            }
        }
    }

    private bool _isAlive = true;
    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
        }
    }

    public bool LockVelocity
    {
        get { return animator.GetBool(AnimationStrings.lockVelocity); }
        set { animator.SetBool(AnimationStrings.lockVelocity, value); }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Health = MaxHealth;
    }

    void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibleTime)
            {
                isInvincible = false;
                timeSinceHit = 0f;
            }
            timeSinceHit += Time.deltaTime;
        }
    }
    public void Initialize(int startingHealth)
    {
        Health = startingHealth;
    }

    public bool TakeDamage(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hit);
            LockVelocity = true;
            hit?.Invoke(damage, knockback);

            CharacterEvents.characterDamaged?.Invoke(gameObject, damage);
            return true;
        }
        return false;
    }

    public bool Heal(int healthRestored)
    {
        if (IsAlive && Health < MaxHealth)
        {
            int maxHeal = MaxHealth - Health;
            int actualHealthRestored = Mathf.Min(maxHeal, healthRestored);
            Health += actualHealthRestored;

            CharacterEvents.characterHealed?.Invoke(gameObject, actualHealthRestored);
            return true;
        }
        return false;
    }
}