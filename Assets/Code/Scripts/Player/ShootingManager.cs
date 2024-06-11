using System;
using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0.1f)] float fire1Ratio = 0.5f;
    [SerializeField, Range(0, 90)] float coneAngle = 30f;
    [SerializeField, Range(0.1f, 2)] float maxDistance = 0.5f;
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

    void Fire1() // Check
    {
        if (!fire1) return;

        if (inputHandler.Fire1Trigger && Time.time > nextFire1)
        {
            nextFire1 = Time.time + fire1Ratio;
            fire1.Play();

            Vector3 coneDirection = transform.forward + Vector3.up;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position + Vector3.up, 0.5f, coneDirection, maxDistance);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer != 6) break;
            }
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
            loading = Time.time;
            Fire?.Invoke(0);
        }
    }
}
