using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    TouchingDirection touchingDirection;

    Vector2 moveInput;

    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float jumpForce = 7f;

    private bool _isMoving = false;
    public bool IsMoving
    {
        get { return _isMoving; }
        set
        {
            if (_isMoving != value)
            {
                animator.SetBool(AnimationStrings.IsMoving, value);
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
                animator.SetBool(AnimationStrings.IsRunning, value);
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
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
        if (_isRunning)
        {
            rb.linearVelocity = new Vector2(moveInput.x * runSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocity.y);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;
        SetFacingDirection(moveInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.IsGround)
        {
            animator.SetTrigger(AnimationStrings.Jump);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    public void OnRunning(InputAction.CallbackContext context)
    {
        if (context.started)
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
            animator.SetTrigger(AnimationStrings.Attack);
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
}
