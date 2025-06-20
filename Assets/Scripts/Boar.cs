using Assets.Scripts;
using UnityEngine;

public class Boar : MonoBehaviour
{
    Rigidbody2D rb;
    LeftSpriteTouchingDirections touchingDirection;
    Animator animator;
    Damageable damageable;

    public GameObject dropItemPrefab;

    public float walkSpeed = 1f;
    public float runSpeed = 4f;
    public float walkStopRate = 0.05f;

    public DetectionZone attackZone;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<LeftSpriteTouchingDirections>();
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isTouchingWallNow = touchingDirection.IsOnWall && touchingDirection.IsGround;

        if (isTouchingWallNow && !wasTouchingWallLastFrame)
        {
            FlipDirection();
        }

        wasTouchingWallLastFrame = isTouchingWallNow;

        if (!damageable.LockVelocity)
        {
            if (CanMove)
            {
                if (HasTarget)
                {
                    rb.linearVelocity = new Vector2(runSpeed * walkDirectionVector.x, rb.linearVelocity.y);
                }
                else
                {
                    rb.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.linearVelocity.y);
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            }
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
        HasTarget = true;
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }
}
