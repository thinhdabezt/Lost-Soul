using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public int CurrentLevel { get; private set; } = 1;

    [Header("Movement")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float jumpForce = 7f;

    [Header("Special Attack")]
    [SerializeField] private GameObject specialAttackPrefab;
    [SerializeField] private Transform specialAttackSpawnPoint;
    [SerializeField] private float specialAttackCooldownTime = 5f;
    private float specialAttackCooldown;

    private Rigidbody2D rb;
    private Animator animator;
    private TouchingDirections touchingDirection;
    private Damageable damageable;
    private ScoreManager scoreManager;

    public Firebase firebaseManager;

    private Vector2 moveInput;
    private bool _isMoving = false;
    private bool _isRunning = false;
    private bool _isFacingRight = true;
    public static bool IsLoadingFromMainMenu = false;
    public bool CanMove => animator.GetBool(AnimationStrings.canMove);
    public bool IsAlive => animator.GetBool(AnimationStrings.isAlive);
    public float AttackCooldown
    {
        get => animator.GetFloat(AnimationStrings.attackCooldown);
        set => animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
    }

    public float CurrentSpeed =>
        CanMove && IsMoving && !touchingDirection.IsOnWall
            ? (_isRunning ? runSpeed : walkSpeed)
            : 0f;

    public bool IsMoving
    {
        get => _isMoving;
        set
        {
            if (_isMoving != value)
                animator.SetBool(AnimationStrings.isMoving, value);
            _isMoving = value;
        }
    }

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning != value)
                animator.SetBool(AnimationStrings.isRunning, value);
            _isRunning = value;
        }
    }

    public bool IsFacingRight
    {
        get => _isFacingRight;
        set
        {
            if (_isFacingRight != value)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _isFacingRight = value;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        firebaseManager = FindAnyObjectByType<Firebase>();
        scoreManager = ScoreManager.Instance;

        if (scene.name.StartsWith("Level"))
        {
            if (int.TryParse(scene.name.Replace("Level", ""), out int level))
                CurrentLevel = level;
            else
                CurrentLevel = 1;
        }
        else
        {
            CurrentLevel = 1;
        }

        if (IsLoadingFromMainMenu)
        {
            LoadDataFromFirebase();
            IsLoadingFromMainMenu = false;
        }
        else if (scene.name.StartsWith("Level"))
        {
            damageable.Initialize(100);
            scoreManager.SetScore(0);
        }
    }

    private void LoadDataFromFirebase()
    {
        if (firebaseManager == null)
            Debug.LogWarning("firebaseManager is null");
        if (scoreManager == null)
            Debug.LogWarning("scoreManager is null");
        if (damageable == null)
            Debug.LogWarning("damageable is null");

        if (firebaseManager == null || scoreManager == null || damageable == null)
        {
            Debug.LogWarning("Missing manager reference.");
            return;
        }

        string username = PlayerPrefs.GetString("Username", "");
        if (string.IsNullOrEmpty(username)) return;

        firebaseManager.LoadPlayerData(username, data =>
        {
            if (data == null)
            {
                damageable.Initialize(100);
                scoreManager.SetScore(0);
                // Không set CurrentLevel ở đây, đã set ở OnSceneLoaded
            }
            else
            {
                damageable.Initialize(data.health);
                scoreManager.SetScore(data.score);
                // Không set CurrentLevel ở đây, đã set ở OnSceneLoaded
            }
        });

    }

    private void Update()
    {
        AttackCooldown -= Time.deltaTime;
        specialAttackCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {
            rb.linearVelocity = new Vector2(moveInput.x * CurrentSpeed, rb.linearVelocity.y);
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            SetFacingDirection(moveInput);
            IsMoving = moveInput.x != 0;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void OnRunning(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.IsGrounded)
            IsRunning = true;
        else if (context.canceled)
            IsRunning = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && AttackCooldown <= 0)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
            AttackCooldown = .5f; // adjust as needed
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.started && specialAttackCooldown <= 0)
        {
            Debug.Log("Special attack triggered");
            animator.SetTrigger(AnimationStrings.specialTrigger);
            specialAttackCooldown = specialAttackCooldownTime;
        }
    }

    private void SpawnSpecialAttack()
    {
        if (specialAttackPrefab != null && specialAttackSpawnPoint != null)
        {
            GameObject attack = Instantiate(specialAttackPrefab, specialAttackSpawnPoint.position, Quaternion.identity);
            attack.transform.localScale = new Vector3(IsFacingRight ? 2 : -2, 2, 2); // face correct direction
        }
    }

    private void SetFacingDirection(Vector2 direction)
    {
        if (direction.x > 0 && !IsFacingRight)
            IsFacingRight = true;
        else if (direction.x < 0 && IsFacingRight)
            IsFacingRight = false;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }
}
