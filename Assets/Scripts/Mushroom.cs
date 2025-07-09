using Assets.Scripts;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    Rigidbody2D rb;
    TouchingDirections touchingDirection;
    Animator animator;
    Damageable damageable;

    public float walkSpeed = 2f;
    public float walkStopRate = 0.05f;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    public enum WalkableDirection
    {
        Left,
        Right
    }

    private bool wasTouchingWallLastFrame = false;

    private Vector2 walkDirectionVector = Vector2.left;

    private WalkableDirection _walkableDirection;

    public WalkableDirection walkableDirection
    {
        get { return _walkableDirection; }
        set
        {
            if (_walkableDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            //if (_walkableDirection != value)
            //{
            //    Vector3 localScale = gameObject.transform.localScale;

            //    if (value == WalkableDirection.Right)
            //        localScale.x = Mathf.Abs(localScale.x);
            //    else
            //        localScale.x = -Mathf.Abs(localScale.x);
            //    gameObject.transform.localScale = localScale;

            //    walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.left : Vector2.right;
            //}

            _walkableDirection = value;
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
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public float AttackCooldown 
    { 
        get 
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        } 
        private set 
        { 
            animator.SetFloat(AnimationStrings.attackCooldown, value);
        } 
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
        
        if(AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isTouchingWallNow = touchingDirection.IsOnWall && touchingDirection.IsGrounded;

        if (isTouchingWallNow && !wasTouchingWallLastFrame || cliffDetectionZone.detectedColliders.Count == 0)
        {
            FlipDirection();
        }

        wasTouchingWallLastFrame = isTouchingWallNow;

        if (!damageable.LockVelocity)
        {
            if (CanMove)
            {
                rb.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    public void FlipDirection()
    {
        if (walkableDirection == WalkableDirection.Left)
        {
            walkableDirection = WalkableDirection.Right;
        }
        else if (walkableDirection == WalkableDirection.Right)
        {
            walkableDirection = WalkableDirection.Left;
        }
    }
    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }
}
