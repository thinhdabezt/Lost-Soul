using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public int CurrentLevel { get; private set; } = 1;
    Rigidbody2D rb;
    Animator animator;
    RightSpriteTouchingDirections touchingDirection;
    Damageable damageable;

    Vector2 moveInput;

    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float jumpForce = 7f;

    private Firebase firebaseManager;
    private ScoreManager scoreManager;

    public float CurrentSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirection.IsOnWall)
                {
                    return IsRunning ? runSpeed : walkSpeed;
                }
                else
                {
                    return 0f;
                }
            }
            else
            {
                return 0f;
            }
        }
    }

    private bool _isMoving = false;
    public bool IsMoving
    {
        get { return _isMoving; }
        set
        {
            if (_isMoving != value)
            {
                animator.SetBool(AnimationStrings.isMoving, value);
            }
            _isMoving = value;
        }
    }

    private bool _isRunning = false;
    public bool IsRunning
    {
        get { return _isRunning; }
        set
        {
            if (_isRunning != value)
            {
                animator.SetBool(AnimationStrings.isRunning, value);
            }
            _isRunning = value;
        }
    }



    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public bool IsAlive
    {
        get { return animator.GetBool(AnimationStrings.isAlive); }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<RightSpriteTouchingDirections>();
        damageable = GetComponent<Damageable>();
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // --- SỬ DỤNG OnEnable VÀ OnDisable ĐỂ QUẢN LÝ SỰ KIỆN ---
    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện khi một scene được tải
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Hủy đăng ký để tránh lỗi
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // --- HÀM NÀY SẼ ĐƯỢC GỌI MỖI KHI MỘT SCENE MỚI ĐƯỢC TẢI XONG ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Khi scene mới được tải, tìm các manager trong scene đó
        FindManagers();
        // Bắt đầu tải dữ liệu người chơi
        LoadDataFromFirebase();
    }

    private void FindManagers()
    {
        firebaseManager = FindAnyObjectByType<Firebase>(); 
        scoreManager = ScoreManager.Instance;
    }

    private void LoadDataFromFirebase()
    {
        if (firebaseManager == null || scoreManager == null || damageable == null)
        {
            Debug.LogWarning("Bỏ qua tải dữ liệu vì thiếu Manager.");
            return;
        }

        string username = PlayerPrefs.GetString("Username", "");
        if (string.IsNullOrEmpty(username)) return;

        firebaseManager.LoadPlayerData(username, (loadedData) =>
        {
            if (loadedData == null)
            {
                Debug.Log("Người chơi mới, sử dụng dữ liệu mặc định.");
                damageable.Initialize(100);
                scoreManager.SetScore(0);
                CurrentLevel = 1; // Level mặc định
            }
            else
            {
                Debug.Log($"Tải dữ liệu thành công: Máu={loadedData.health}, Điểm={loadedData.score}, Level={loadedData.level}");
                damageable.Initialize(loadedData.health);
                scoreManager.SetScore(loadedData.score);
                CurrentLevel = loadedData.level; // Lấy level từ Firebase
            }
        });
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created  
    void Start()
    {

    }

    // Update is called once per frame  
    void Update()
    {

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
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    private void SetFacingDirection(Vector2 direction)
    {
        if (direction.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (direction.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }
}
