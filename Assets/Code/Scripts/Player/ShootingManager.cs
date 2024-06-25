using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
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

    [Header("VFX")]
    [SerializeField, Tooltip("VFX while loading second attack")] ParticleSystem loadingVFX;

    [Header("SFX")]
    [SerializeField, Tooltip("SFX first attack")] AudioClip fire1SFX;
    [SerializeField, Tooltip("SFX second attack")] AudioClip fire2SFX;
    [SerializeField, Tooltip("SFX while loading second attack")] AudioClip loadingSFX;

    private InputHandler inputHandler;
    private AudioSource audioSource;

    private float nextFire1 = 0f;
    private float nextFire2 = 0f;
    private float loading;

    private bool isSetted = false;

    public delegate void FireLoading(float value);
    public static event FireLoading Fire;
    public static event FireLoading Load1;
    public static event FireLoading Load2;

#if UNITY_EDITOR

    private void OnValidate()
    {
        // VFX
        if (!fire1) Debug.LogWarning("fire1 not assigned");
        if (!fire2) Debug.LogWarning("fire2 not assigned");
        if (!loadingVFX) Debug.LogWarning("loadingVFX not assigned");
        
        // SFX
        if (!fire1SFX) Debug.LogWarning("fire1SFX not assigned");
        if (!fire2SFX) Debug.LogWarning("fire2SFX not assigned");
        if (!loadingSFX) Debug.LogWarning("loadingSFX not assigned");
    }

#endif

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        inputHandler = InputHandler.Instance;
        Fire?.Invoke(timeLoading);
        isSetted = true;
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

        Load1?.Invoke(Time.time - nextFire1);

        if (inputHandler.Fire1Trigger && Time.time > nextFire1)
        {
            nextFire1 = Time.time + fire1Ratio;
            fire1.Play();

            if (audioSource && fire1SFX && !audioSource.isPlaying) audioSource.PlayOneShot(fire1SFX);
        }
    }

    void Fire2()
    {
        if (!fire2)
        {
            Debug.LogWarning("No fire 2 particle system found");
            return;
        }
        Load2?.Invoke(Time.time - nextFire2);

        if (inputHandler.Fire2Trigger && Time.time > nextFire2)
        {
            if (loadingVFX && !loadingVFX.isPlaying) loadingVFX.Play();
            if (audioSource && loadingSFX && !audioSource.isPlaying)
            {
                audioSource.clip = loadingSFX;
                audioSource.Play();
            }

            Fire?.Invoke(Time.time - loading);

            if (Time.time - loading >= timeLoading)
            {
                if (loadingVFX) loadingVFX.Stop();

                nextFire2 = Time.time + fire2Ratio;
                //fire2.Emit(1);

                fire2.Stop();
                fire2.Play();

                if (audioSource && fire2SFX) audioSource.PlayOneShot(fire2SFX);

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
        if (!isSetted) return;

        if (loadingVFX) loadingVFX.Stop();

        loading = Time.time;
        Fire?.Invoke(0);
        audioSource.Stop();
    }
}
