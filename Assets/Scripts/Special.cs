using UnityEngine;

public class Special : MonoBehaviour
{
    public int damage = 25;
    public float speed = 9f;
    public Vector2 knockback = new Vector2(3f, 1f);

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.linearVelocity = new Vector2(speed * transform.localScale.x, 0);
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
        if (damageable != null && damageable.IsAlive)
        {
            damageable.TakeDamage(damage, deliveredKnockback);
            Destroy(gameObject);
        }
    }
}
