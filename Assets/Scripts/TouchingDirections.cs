using Assets.Scripts;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    private CapsuleCollider2D capsuleCollider2d;
    private Animator animator;

    public ContactFilter2D contactFilter;

    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

    public float groundCheckDistance = 0.1f;
    public float wallCheckDistance = 0.2f;
    public float ceilingCheckDistance = 0.05f;

    public enum WallCheckFacing { LeftFacingIsRight, RightFacingIsRight }
    public WallCheckFacing facingMode = WallCheckFacing.RightFacingIsRight;

    private Vector2 WallCheckDirection
    {
        get
        {
            bool facingRight = transform.localScale.x > 0;

            if (facingMode == WallCheckFacing.RightFacingIsRight)
                return facingRight ? Vector2.right : Vector2.left;
            else
                return facingRight ? Vector2.left : Vector2.right;
        }
    }

    private bool _isGrounded = true;
    public bool IsGrounded
    {
        get => _isGrounded;
        set
        {
            if (_isGrounded != value)
                animator.SetBool(AnimationStrings.isGrounded, value);
            _isGrounded = value;
        }
    }

    private bool _isOnWall = true;
    public bool IsOnWall
    {
        get => _isOnWall;
        set
        {
            if (_isOnWall != value)
                animator.SetBool(AnimationStrings.isOnWall, value);
            _isOnWall = value;
        }
    }

    private bool _isOnCeiling = true;
    public bool IsOnCeiling
    {
        get => _isOnCeiling;
        set
        {
            if (_isOnCeiling != value)
                animator.SetBool(AnimationStrings.isOnWall, value);
            _isOnCeiling = value;
        }
    }

    private void Awake()
    {
        capsuleCollider2d = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        IsGrounded = capsuleCollider2d.Cast(Vector2.down, contactFilter, groundHits, groundCheckDistance) > 0;
        IsOnWall = capsuleCollider2d.Cast(WallCheckDirection, contactFilter, wallHits, wallCheckDistance) > 0;
        IsOnCeiling = capsuleCollider2d.Cast(Vector2.up, contactFilter, ceilingHits, ceilingCheckDistance) > 0;
    }
}
