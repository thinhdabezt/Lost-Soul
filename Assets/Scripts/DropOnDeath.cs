using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    public GameObject dropPrefab;

    public void Drop()
    {
        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }
    }
}
