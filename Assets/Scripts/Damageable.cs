using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> Hit;
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
            if(_health <= 0)
            {
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

            Hit?.Invoke(damage, knockback);

            return true;
        }
        return false;
    }
}
