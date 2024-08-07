using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
public class Enemy2Controller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0), Tooltip("Delay before start")] float startDelay = 0f;
    [SerializeField, Min(0f), Tooltip("Tracking speed")] float speed = 1f;
    [SerializeField, Min(0f), Tooltip("Delay before impact")] float impactDelay = 1f;
    [SerializeField, Min(0f), Tooltip("Ratio of time between each shot")] float fireRatio = 1f;

    [Header("Refereces")]
    [SerializeField] List<MissileLauncher> missileLauncher = new();
    [SerializeField] List<ParticleSystem> particleSystems = new();

    [Header("VFX")]
    [SerializeField] Transform body;

    [Header("SFX")]
    [SerializeField] AudioClip explosion;

    private AudioSource _audioSource;
    private Transform target;
    private float timeSinceLastFire = 0f;
    private bool isTargetting = false;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (particleSystems.Count <= 0) Debug.LogWarning("particleSystems not assigned");
        if (!body) Debug.LogWarning("body not assigned");
    }

#endif

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //target = Move.Instance.transform;
        target = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (Time.time < startDelay) return;

        if (!isTargetting)
        {
            transform.position = Vector3.MoveTowards(transform.position, new(target.position.x, transform.position.y, target.position.z), speed * Time.deltaTime);
        }

        if (Time.time - timeSinceLastFire > fireRatio + impactDelay)
        {
            timeSinceLastFire = Time.time;
            isTargetting = true;

            // VFX
            foreach (var missile in missileLauncher)
            {
                missile.Shoot(fireRatio);
            }

            StartCoroutine(Targeting(impactDelay + (fireRatio * 0.7f)));

            StartCoroutine(Shooting(impactDelay));
        }
    }

    IEnumerator Targeting(float delay = 0f)
    {
        if (body) body.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(delay);

        if (body) body.gameObject.SetActive(false);

        isTargetting = false;
    }

    IEnumerator Shooting(float delay = 0f)
    {
        yield return new WaitForSecondsRealtime(delay * 0.5f);

        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Emit(1);
        }

        yield return new WaitForSecondsRealtime(0.1f);

        if (_audioSource && explosion) _audioSource.PlayOneShot(explosion);
    }
}
