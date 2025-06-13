using Assets.Scripts;
using UnityEngine;

public class TouchingDirection : MonoBehaviour
{
    CapsuleCollider2D capsuleCollider2d;
    Animator animator;

    public ContactFilter2D contactFilter;

    RaycastHit2D[] groundHits = new RaycastHit2D[10];
    RaycastHit2D[] wallHits = new RaycastHit2D[10];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[10];

    public float groundCheckDistance = 0.1f;
    public float wallCheckDistance = 0.2f;
    public float ceilingCheckDistance = 0.05f;

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    private bool _isGrounded = true;

    public bool IsGround 
    { get 
        {
            return _isGrounded;
        }
        set
        {
            if (_isGrounded != value)
            {
                _isGrounded= value;
                animator.SetBool(AnimationStrings.IsGrounded, value);
            }
            _isGrounded = value;
        }
    }

    private bool _isOnWall = true;

    public bool IsOnWall
    {
        get
        {
            return _isOnWall;
        }
        set
        {
            if (_isOnWall != value)
            {
                _isOnWall = value;
                animator.SetBool(AnimationStrings.IsOnWall, value);
            }
            _isOnWall = value;
        }
    }

    private bool _isOnCeiling = true;

    public bool IsOnCeiling
    {
        get
        {
            return _isOnCeiling;
        }
        set
        {
            if (_isOnCeiling != value)
            {
                _isOnCeiling = value;
                animator.SetBool(AnimationStrings.IsOnWall, value);
            }
            _isOnCeiling = value;
        }
    }

    private void Awake()
    {
        capsuleCollider2d = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        IsGround = capsuleCollider2d.Cast(Vector2.down, contactFilter, groundHits, groundCheckDistance) > 0;
        IsOnWall = capsuleCollider2d.Cast(wallCheckDirection, contactFilter, wallHits, wallCheckDistance) > 0;
        IsOnCeiling = capsuleCollider2d.Cast(Vector2.up, contactFilter, ceilingHits, ceilingCheckDistance) > 0;
    }
}
