using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    public GameObject specialAttackPrefab;

    public void PerformSpecialAttack(Vector2 position, Vector2 direction)
    {
        Instantiate(specialAttackPrefab, transform.position, specialAttackPrefab.transform.rotation);
    }
}
