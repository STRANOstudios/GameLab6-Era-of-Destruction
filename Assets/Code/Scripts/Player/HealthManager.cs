using System;
using Unity.AI.Navigation;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(1f)] float health = 100f;
    [SerializeField, Tooltip("Layer mask of objects that harm us")] LayerMask layerMask;

    [Header("VFX")]
    [SerializeField, Tooltip("VFX of damage taken")] ParticleSystem VFXDamage;

    [Header("SFX")]
    [SerializeField, Tooltip("SFX of damage taken")] AudioClip damageSFX;

    private AudioSource audioSource;

    public delegate void HealthListener(float health);
    public static event HealthListener HealthValue;

    public static Action playerDeath;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if(layerMask == 0) Debug.LogWarning("layerMask not assigned");

        // VFX
        if (!VFXDamage) Debug.LogWarning("VFXDamage not assigned");

        // SFX
        if (!damageSFX) Debug.LogWarning("damageSFX not assigned");
    }

#endif

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        HealthValue?.Invoke(health);
    }

    private void OnParticleCollision(GameObject other)
    {
        if ((layerMask.value & (1 << other.layer)) != 0)
        {
            if (audioSource && damageSFX) audioSource.PlayOneShot(damageSFX);

            health -= other.GetComponent<Damage>().GetDamage;

            if (VFXDamage)
            {
                VFXDamage.Stop();
                VFXDamage.Play();
            }

            HealthValue?.Invoke(health); // Update UI

            if (health <= 0f)
            {
                playerDeath?.Invoke(); // Notify player death
            }
        }
    }
}
