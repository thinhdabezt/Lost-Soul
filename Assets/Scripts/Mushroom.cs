using UnityEngine;

public class Mushroom : MonoBehaviour
{
    Rigidbody2D rb;

    public float walkSpeed = 2f;

    public enum WalkableDirection
    {
        Left,
        Right
    }

    private WalkableDirection _walkableDirection;

    public WalkableDirection walkableDirection
    {
        get { return _walkableDirection; }
        set
        {
            if (_walkableDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, transform.localScale.y);

                if (value == WalkableDirection.Left)
                {
                    rb.linearVelocity = new Vector2(-walkSpeed, rb.linearVelocity.y);
                }
                else if (value == WalkableDirection.Right)
                {
                    rb.linearVelocity = new Vector2(walkSpeed, rb.linearVelocity.y);
                }
            }

            _walkableDirection = value;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(-walkSpeed, rb.linearVelocity.y);
    }
}
