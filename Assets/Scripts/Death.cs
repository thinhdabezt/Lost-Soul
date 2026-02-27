using Assets.Scripts;
using UnityEngine;

public class Death : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Animator animator;
    private Damageable damageable;

    [Header("Detection")]
    public DetectionZone attackZone;
    public DetectionZone playerZone;
    public DetectionZone specialZone;
    public DetectionZone cliffZone;
    public LayerMask whatIsPlayer;

    [Header("Movement")]
    public float walkAcceleration = 100f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.1f;

    [Header("Special Attack")]
    public GameObject specialAttackPrefab;
    public Transform specialAttackSpawnPoint;
    [SerializeField] private float specialCooldown = 4f;
    private float lastSpecialTime = -999f;

    private Transform targetPlayer;

    public enum WalkableDirection { Left, Right }
    private WalkableDirection _walkDirection = WalkableDirection.Right;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get => _walkDirection;
        set
        {
            if (_walkDirection != value)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (value == WalkableDirection.Right ? 1 : -1);
                transform.localScale = scale;

                walkDirectionVector = value == WalkableDirection.Right ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    private bool isMoving;
    public bool IsMoving
    {
        get => isMoving;
        set
        {
            isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    private bool hasTarget;
    public bool HasTarget
    {
        get => hasTarget;
        set
        {
            hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    private bool CanMove => animator.GetBool(AnimationStrings.canMove);

    private float AttackCooldown
    {
        get => animator.GetFloat(AnimationStrings.attackCooldown);
        set => animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(0, value));
    }

    private bool IsNearCliff => cliffZone != null && cliffZone.detectedColliders.Count == 0;
    private bool CanUseSpecial => Time.time >= lastSpecialTime + specialCooldown;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    private void Update()
    {
        // Update target detection
        HasTarget = attackZone.detectedColliders.Count > 0;

        if (playerZone.detectedColliders.Count > 0)
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

        if (AttackCooldown > 0f)
            AttackCooldown -= Time.deltaTime;

        // Special Attack
        if (specialZone.detectedColliders.Count > 0 && CanUseSpecial && !HasTarget)
        {
            animator.SetTrigger(AnimationStrings.specialTrigger);
            lastSpecialTime = Time.time;
        }
    }

    private void FixedUpdate()
    {
        bool mustFlip = IsNearCliff || (touchingDirections.IsOnWall && touchingDirections.IsGrounded);

        // PRIORITY: Flip first if needed, even if chasing
        if (mustFlip)
        {
            FlipDirection();
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            IsMoving = false;
            return;
        }

        if (!damageable.LockVelocity && touchingDirections.IsGrounded && CanMove)
        {
            float moveDir = walkDirectionVector.x;

            if (targetPlayer != null)
            {
                moveDir = Mathf.Sign(targetPlayer.position.x - transform.position.x);
                WalkDirection = moveDir > 0 ? WalkableDirection.Right : WalkableDirection.Left;
            }

            Vector2 velocity = new Vector2(
                Mathf.Clamp(rb.linearVelocity.x + walkAcceleration * moveDir * Time.fixedDeltaTime, -maxSpeed, maxSpeed),
                rb.linearVelocity.y
            );

            rb.linearVelocity = velocity;
            IsMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        }
        else if (!CanMove)
        {
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            IsMoving = false;
        }
    }

    public void PerformSpecialAttack()
    {
        GameObject special = Instantiate(specialAttackPrefab, specialAttackSpawnPoint.position, Quaternion.identity);
        special.transform.localScale = new Vector3(transform.localScale.x * 2f, 2f, 1f);
    }

    private void FlipDirection()
    {
        WalkDirection = WalkDirection == WalkableDirection.Right ? WalkableDirection.Left : WalkableDirection.Right;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }
}
