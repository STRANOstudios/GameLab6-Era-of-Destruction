using System;
using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0.1f)] float fire1Ratio = 0.5f;
    [Space]
    [SerializeField, Min(0.1f)] float fire2Ratio = 0.5f;
    [SerializeField, Min(0f)] float timeLoading = 0.5f;

    [Header("References")]
    [SerializeField] ParticleSystem fire1;
    [SerializeField] ParticleSystem fire2;

    private InputHandler inputHandler;

    private float nextFire1 = 0f;
    private float nextFire2 = 0f;
    private float loading;

    public delegate void FireLoading(float value);
    public static event FireLoading Fire;

    void Start()
    {
        inputHandler = InputHandler.Instance;
        Fire?.Invoke(timeLoading);
    }

    void Update()
    {
        Fire1();
        Fire2();
    }

    private void OnEnable()
    {
        HealthManager.HealthValue += OnHitDetected;
        Move.OnMove += ResetLoding;
    }

    private void OnDisable()
    {
        HealthManager.HealthValue -= OnHitDetected;
        Move.OnMove += ResetLoding;
    }

    void Fire1()
    {
        if (!fire1) return;

        if (inputHandler.Fire1Trigger && Time.time > nextFire1)
        {
            nextFire1 = Time.time + fire1Ratio;
            fire1.Play();
        }
    }

    void Fire2() // creare caricamento to be testing
    {
        if (!fire2) return;

        if (inputHandler.Fire2Trigger && Time.time > nextFire2)
        {
            Fire?.Invoke(Time.time - loading);

            if (Time.time - loading >= timeLoading)
            {
                nextFire2 = Time.time + fire2Ratio;
                fire2.Emit(1);

                loading = Time.time;
            }
        }
        else
        {
            ResetLoding();
        }
    }

    private void OnHitDetected(float value = 0f)
    {
        ResetLoding();
    }

    private void ResetLoding()
    {
        loading = Time.time;
        Fire?.Invoke(0);
    }
}
