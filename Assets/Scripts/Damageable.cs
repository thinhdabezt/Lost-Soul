using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> hit;
    public UnityEvent<int, int> healthChanged;
    public UnityEvent OnPlayerDeath;

    public enum DamageableType { Player, Enemy }

    public DamageableType damageableType;

    Animator animator;

    private bool isInvincible = false;
    private float timeSinceHit = 0;

    [SerializeField] private float invincibleTime = 0.25f;
    [SerializeField] private int _maxHealth;
    public int MaxHealth
    {
        get 
        { 
            return _maxHealth; 
        }
        set 
        { 
            _maxHealth = Mathf.Max(0, value); 
        } 
    }

    [SerializeField] private int _health;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = Mathf.Max(0, value);
            healthChanged?.Invoke(_health, MaxHealth);
            if (_health <= 0)
            {
                if (damageableType == DamageableType.Player)
                {
                    OnPlayerDeath?.Invoke();
                }

                IsAlive = false;
            }
        }
    }

    private bool _isAlive = true;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
        }
    }

    public bool LockVelocity
    {
        get 
        { 
            return animator.GetBool(AnimationStrings.lockVelocity); 
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
        if (IsAlive && Health != MaxHealth)
        {
            int maxHealth = Mathf.Max(MaxHealth - Health, 0);
            int actualHealthRestored = Mathf.Min(maxHealth, healthRestored);
            Health += actualHealthRestored;

            CharacterEvents.characterHealed?.Invoke(gameObject, actualHealthRestored);

            return true;
        }

        return false;
    }
}
