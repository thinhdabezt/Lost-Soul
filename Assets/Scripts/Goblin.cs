using Assets.Scripts;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    public DetectionZone attackZone;
    public DetectionZone cliffZone;

    public float walkAcceleration = 100f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.1f;

    public enum WalkableDirection
    {
        Left,
        Right
    }

    private WalkableDirection _walkDirection;

    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get
        {
            return _walkDirection;
        }
        set
        {
            if (_walkDirection != value)
            {
                Vector3 localScale = gameObject.transform.localScale;
                if (value == WalkableDirection.Right)
                    localScale.x = Mathf.Abs(localScale.x);
                else
                    localScale.x = -Mathf.Abs(localScale.x);
                gameObject.transform.localScale = localScale;

                walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    public bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }

        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get
        {
            return _hasTarget;
        }

        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    void FixedUpdate()
    {
        if (touchingDirections.IsOnWall && touchingDirections.IsGrounded || cliffZone.detectedColliders.Count == 0)
        {
            FlipDirection();
        }
        if (!damageable.LockVelocity && touchingDirections.IsGrounded)
        {
            if (CanMove)
            {
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.linearVelocity.y);
                IsMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
                IsMoving = false;
            }
        }
    }

    private void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);

    }
}
