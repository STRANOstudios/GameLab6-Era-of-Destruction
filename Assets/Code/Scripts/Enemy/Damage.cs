using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] float damage = 10f;

    public float GetDamage => damage;
}
