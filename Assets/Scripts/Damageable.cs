using Assets.Scripts;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Damageable : MonoBehaviour
{
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
        }
    }

    private bool _isAlive;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.IsAlive, value);
            //Debug.Log($"IsAlive set to: {value}");
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
                timeSinceHit = 0f; // Reset the timer after invincibility ends
            }

            timeSinceHit += Time.deltaTime; // Increment the timer
        }
        //Debug.Log($"IsAlive set to: {_health}");
        TakeDamage(20);
    }

    public void TakeDamage(int damage)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
            if (Health <= 0)
            {
                IsAlive = false;
            }
        }
    }
}
