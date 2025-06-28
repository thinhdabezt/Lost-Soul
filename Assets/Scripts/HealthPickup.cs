using UnityEngine;
using UnityEngine.Audio;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 20;
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable)
        {
            bool wasHealed = damageable.Heal(healthAmount);
            if (wasHealed)
            {
                Destroy(gameObject);
            }
        }
    }
}
