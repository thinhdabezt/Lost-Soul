using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has a Damageable component
        Damageable damageable = collision.GetComponent<Damageable>();
        Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
        if (damageable != null && damageable.IsAlive)
        {
            damageable.TakeDamage(attackDamage, deliveredKnockback);
        }
    }
}
