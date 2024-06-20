using UnityEngine;

[DisallowMultipleComponent]
public class ShootingManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0.1f), Tooltip("Is the ratio of first attack")] float fire1Ratio = 0.5f;
    [Space]
    [SerializeField, Min(0.1f), Tooltip("Is the ratio of second attack")] float fire2Ratio = 0.5f;
    [SerializeField, Min(0f), Tooltip("Is the time to load second attack")] float timeLoading = 0.5f;

    [Header("References")]
    [SerializeField, Tooltip("First attack")] ParticleSystem fire1;
    [SerializeField, Tooltip("Second attack")] ParticleSystem fire2;
    [SerializeField, Tooltip("VFX while loading second attack")] ParticleSystem loadingVFX;

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

        MoveNightmare.OnMove += ResetLoding;
    }

    private void OnDisable()
    {
        HealthManager.HealthValue -= OnHitDetected;
        Move.OnMove -= ResetLoding;

        MoveNightmare.OnMove -= ResetLoding;
    }

    void Fire1()
    {
        if (!fire1)
        {
            Debug.LogWarning("No fire 1 particle system found");
            return;
        }

        if (inputHandler.Fire1Trigger && Time.time > nextFire1)
        {
            nextFire1 = Time.time + fire1Ratio;
            fire1.Play();
        }
    }

    void Fire2() // creare caricamento to be testing
    {
        if (!fire2)
        {
            Debug.LogWarning("No fire 2 particle system found");
            return;
        }

        if (inputHandler.Fire2Trigger && Time.time > nextFire2)
        {
            if (loadingVFX && !loadingVFX.isPlaying) loadingVFX.Play();

            Fire?.Invoke(Time.time - loading);

            if (Time.time - loading >= timeLoading)
            {
                if (loadingVFX) loadingVFX.Stop();

                nextFire2 = Time.time + fire2Ratio;
                //fire2.Emit(1);

                fire2.Stop();
                fire2.Play();

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
        if (loadingVFX) loadingVFX.Stop();

        loading = Time.time;
        Fire?.Invoke(0);
    }
}
