using System;
using Unity.AI.Navigation;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(1f)] float health = 100f;
    [SerializeField] LayerMask layerMask;

    [Header("References")]
    [SerializeField] ParticleSystem VFXDamage;

    public delegate void HealthListener(float health);
    public static event HealthListener HealthValue;

    public static Action playerDeath;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!VFXDamage) Debug.LogWarning("VFXDamage not assigned");
    }

#endif

    private void Start()
    {
        HealthValue?.Invoke(health);
    }

    private void OnParticleCollision(GameObject other)
    {
        if ((layerMask.value & (1 << other.layer)) != 0)
        {
            health -= other.GetComponent<Damage>().GetDamage;

            VFXDamage.Stop();
            VFXDamage.Play();

            HealthValue?.Invoke(health); // Update UI

            if (health <= 0f)
            {
                playerDeath?.Invoke(); // Notify player death
            }
        }
    }
}
