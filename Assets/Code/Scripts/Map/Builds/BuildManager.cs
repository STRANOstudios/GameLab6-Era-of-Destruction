using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Collider))]
public class BuildManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0.1f), Tooltip("health of the build")] float health = 100f;
    [SerializeField, Min(0), Tooltip("points earned by destroying the palace")] int score = 100;
    [SerializeField, Min(0f), Tooltip("seconds to add to the timer")] float sec = 10f;
    [Space]
    [SerializeField, Min(0f), Tooltip("height of the build")] float height = 1f;

    [SerializeField, Range(0f, 20f), Tooltip("seconds to wait before to build")] float timeToBuild = 5f;
    [Space]
    [SerializeField] LayerMask layerMask;

    [Header("References")]
    [SerializeField] ParticleSystem destructionParticles;
    [SerializeField] ParticleSystem reconstructionParticles;

    [Header("VFX")]
    [SerializeField, Tooltip("mesh of the build after collapse")] Mesh rubbleMesh;
    [SerializeField, Min(0.1f), Tooltip("time of VFX collapse")] float timeVFXCollapse = 0.5f;
    [SerializeField, Min(0.1f), Tooltip("time of VFX build")] float timeVFXSpawn = 0.5f;
    [SerializeField, Min(0f), Tooltip("speed of collapse")] float collapseSpeed = 1f;

    [Header("SFX")]
    [SerializeField] AudioClip sfxDestruction;
    [SerializeField] AudioClip sfxReconstruction;

    private Collider _collider;
    private Material material;
    private Transform _transformMesh;
    private Mesh defaultMesh;
    private MeshFilter meshFilter;
    private AudioSource _audioSource;
    private float _startHealth;
    private Vector3 _spawnPosition;

    public delegate void Score(float score);
    public static event Score OnScore;
    public static event Score OnSec;

#if UNITY_EDITOR

    private void OnValidate()
    {
        // VFX
        if (!destructionParticles) Debug.LogWarning("destructionParticles not assigned");
        if (!reconstructionParticles) Debug.LogWarning("reconstructionParticles not assigned");
        if (!rubbleMesh) Debug.LogWarning("rubbleMesh not assigned");

        // SFX
        if (!sfxDestruction) Debug.LogWarning("sfxDestruction not assigned");
        if (!sfxReconstruction) Debug.LogWarning("sfxReconstruction not assigned");
    }

#endif

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
        _startHealth = health;
        _spawnPosition = transform.position;
        meshFilter = GetComponentInChildren<MeshFilter>();
        defaultMesh = meshFilter.sharedMesh;
    }

    private void Start()
    {
        material = GetComponentInChildren<MeshRenderer>().material;
        _transformMesh = transform.GetChild(0);
    }

    private void OnParticleCollision(GameObject other)
    {
        if ((layerMask.value & (1 << other.layer)) != 0)
        {
            health -= other.GetComponent<Damage>().GetDamage;

            if (health <= 0) StartCoroutine(HandleDestructionReconstructionVFX());
        }
    }

    private IEnumerator HandleDestructionReconstructionVFX()
    {
        #region destruction

        OnScore?.Invoke((float)score);
        OnSec?.Invoke(sec);

        if (destructionParticles) destructionParticles.Play();
        if (_audioSource && sfxDestruction) _audioSource.PlayOneShot(sfxDestruction);

        StartCoroutine(Collapse(collapseSpeed));
        StartCoroutine(FadeInOut(timeVFXCollapse));

        if (_collider) _collider.enabled = false;

        yield return new WaitForSeconds(timeVFXCollapse);

        if (destructionParticles) destructionParticles.Stop();

        //ChangeFade(255f);
        //ChangeMesh(rubbleMesh);

        #endregion

        yield return new WaitForSeconds(timeToBuild);

        #region reconstruction

        if (reconstructionParticles) reconstructionParticles.Play();
        if (_audioSource && sfxReconstruction) _audioSource.PlayOneShot(sfxReconstruction);

        if (_collider) _collider.enabled = true;

        //ChangeMesh(defaultMesh);
        //ChangeFade(0f);

        // Fade In
        yield return StartCoroutine(FadeInOut(timeVFXSpawn));

        if (reconstructionParticles) reconstructionParticles.Stop();

        _transformMesh.position = _spawnPosition;

        health = _startHealth;

        #endregion
    }

    private void ChangeMesh(Mesh mesh)
    {
        meshFilter.sharedMesh = mesh;
        //GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }

    private void ChangeFade(float alpha)
    {
        Color newColor = material.color;
        newColor.a = alpha;
        material.color = newColor;
    }

    private IEnumerator FadeInOut(float duration)
    {
        float startAlpha = material.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, startAlpha == 0f ? 1f : 0f, elapsed / duration);
            Color newColor = material.color;
            newColor.a = newAlpha;
            material.color = newColor;
            yield return null;
        }
    }

    private IEnumerator Collapse(float speed)
    {
        float duration = height / speed;
        float startY = _transformMesh.position.y;
        float targetY = startY - height;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float y = Mathf.Lerp(startY, targetY, t);
            _transformMesh.position = new Vector3(_spawnPosition.x, y, _spawnPosition.z);
            yield return null;
        }
    }
}
