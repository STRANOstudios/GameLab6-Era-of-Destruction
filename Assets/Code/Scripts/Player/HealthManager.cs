using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(1f)] float health = 100f;
    [SerializeField] LayerMask layerMask;

    public delegate void HealthListener(float health);
    public static event HealthListener HealthValue;

    public Action playerDeath;

    private void Start()
    {
        HealthValue?.Invoke(health);
    }

    private void OnParticleCollision(GameObject other)
    {
        if ((layerMask.value & (1 << other.layer)) != 0)
        {
            health -= other.GetComponent<Damage>().GetDamage;

            HealthValue?.Invoke(health); // Update UI

            if (health <= 0f)
            {
                playerDeath?.Invoke(); // Notify player death
            }
        }
    }
}
