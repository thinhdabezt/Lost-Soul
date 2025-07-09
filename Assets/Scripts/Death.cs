using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class Death : MonoBehaviour
{
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    public DetectionZone attackZone;
    public DetectionZone playerZone;
    public DetectionZone specialZone;
    public DetectionZone cliffZone;

    public LayerMask whatIsPlayer;
    public Transform playerCheck;
    public float playerCheckDistance = 10f;

    public float walkAcceleration = 100f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.1f;
    [SerializeField] private float specialCooldown = 4f;
    [SerializeField] private float lastSpecialTime = 0;
    public GameObject specialAttackPrefab;
    public Transform specialAttackSpawnPoint;

    [SerializeField] private Transform targetPlayer;

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
                // If player detected, chase them
                if (targetPlayer != null)
                {
                    float direction = Mathf.Sign(targetPlayer.position.x - transform.position.x);
                    WalkDirection = direction > 0 ? WalkableDirection.Right : WalkableDirection.Left;
                    rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * direction * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.linearVelocity.y);
                    IsMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
                }
                else
                {
                    rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.linearVelocity.y);
                    IsMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
                IsMoving = false;
            }
        }
    }

    bool cooldownReady => Time.time >= lastSpecialTime + specialCooldown;

    private void Update()
    {
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

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

        if (specialZone.detectedColliders.Count > 0 && cooldownReady && !HasTarget)
        {
            animator.SetTrigger(AnimationStrings.specialTrigger);
            lastSpecialTime = Time.time;
        }
    }

    public void PerformSpecialAttack()
    {
        GameObject special = Instantiate(specialAttackPrefab, specialAttackSpawnPoint.position, transform.rotation);
        special.transform.localScale = transform.localScale * 2;
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
