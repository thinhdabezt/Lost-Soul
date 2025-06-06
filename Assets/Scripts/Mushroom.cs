using UnityEngine;

public class Mushroom : MonoBehaviour
{
    Rigidbody2D rb;

    public float walkSpeed = 2f;

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
