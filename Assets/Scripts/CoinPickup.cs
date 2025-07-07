using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    [SerializeField] private int coinValue = 1;

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
        ScoreManager.Instance.AddScore(coinValue);
        Destroy(gameObject);
    }
}
