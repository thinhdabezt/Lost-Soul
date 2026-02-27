using UnityEngine;
using Assets.Scripts;

public class BasicMonster : MonoBehaviour
{
    [Header("Components")]
    protected Rigidbody2D rb;
    protected TouchingDirections touchingDirections;
    protected Animator animator;
    protected Damageable damageable;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float walkAcceleration = 100f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.1f;
    public bool isConstantSpeed = false;

    [Header("Detection")]
    public DetectionZone attackZone;
    public DetectionZone cliffZone;
    public DetectionZone playerZone;
    public LayerMask whatIsPlayer;

    protected Transform targetPlayer;

    public enum WalkableDirection { Left, Right }
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get => _walkDirection;
        set
        {
            if (_walkDirection != value)
            {
                Vector3 localScale = transform.localScale;
                localScale.x = Mathf.Abs(localScale.x) * (value == WalkableDirection.Right ? 1 : -1);
                transform.localScale = localScale;

                walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    public bool canMove => animator.GetBool(AnimationStrings.canMove);
    public bool isMoving
    {
        get => animator.GetBool(AnimationStrings.isMoving);
        set => animator.SetBool(AnimationStrings.isMoving, value);
    }

    public float attackCooldown
    {
        get => animator.GetFloat(AnimationStrings.attackCooldown);
        set => animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (playerZone != null && playerZone.detectedColliders.Count > 0)
        {
            foreach (var col in playerZone.detectedColliders)
            {
                if (((1 << col.gameObject.layer) & whatIsPlayer) != 0)
                {
                    targetPlayer = col.transform;
                    break;
                }
            }
        }
        else
        {
            targetPlayer = null;
        }

        bool hasTarget = attackZone != null && attackZone.detectedColliders.Count > 0;
        animator.SetBool(AnimationStrings.hasTarget, hasTarget);
    }

    public float stopDistance = 1.0f; // Add this to the Movement header for easy tuning

    private void FixedUpdate()
    {
        bool isTouchingWall = touchingDirections.IsOnWall && touchingDirections.IsGrounded;
        bool isNearCliff = cliffZone != null && cliffZone.detectedColliders.Count == 0;

        // Priority: FLIP at cliff or wall if NOT chasing
        if ((isTouchingWall || isNearCliff) && targetPlayer == null)
        {
            FlipDirection();
        }

        // Prevent falling off cliff even if chasing
        if (isNearCliff)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // stop moving
            isMoving = false;
            return;
        }

        if (!damageable.LockVelocity && touchingDirections.IsGrounded && canMove)
        {
            float direction = walkDirectionVector.x;

            // Only chase player if no cliff ahead
            if (targetPlayer != null)
            {
                float distanceToPlayer = Mathf.Abs(targetPlayer.position.x - transform.position.x);

                if (distanceToPlayer <= stopDistance)
                {
                    // Stop near the player
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                    isMoving = false;
                    return;
                }

                direction = Mathf.Sign(targetPlayer.position.x - transform.position.x);
                WalkDirection = direction > 0 ? WalkableDirection.Right : WalkableDirection.Left;
            }

            Vector2 newVelocity;
            if (isConstantSpeed)
            {
                newVelocity = new Vector2(walkSpeed * direction, rb.linearVelocity.y);
            }
            else
            {
                newVelocity = new Vector2(
                    Mathf.Clamp(rb.linearVelocity.x + walkAcceleration * direction * Time.fixedDeltaTime, -maxSpeed, maxSpeed),
                    rb.linearVelocity.y
                );
            }

            rb.linearVelocity = newVelocity;
            isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        }
        else if (!canMove)
        {
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            isMoving = false;
        }
    }


    public void FlipDirection()
    {
        WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkableDirection.Right;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }
}