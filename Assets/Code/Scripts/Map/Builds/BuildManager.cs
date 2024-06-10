using System.Collections;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Collider))]
public class BuildManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0.1f)] float health = 100f;
    [SerializeField, Min(0f)] float score = 100f;

    [SerializeField, Range(0f, 20f)] float timeToBuild = 5f;

    [Header("References")]
    [SerializeField] ParticleSystem destructionParticles;
    [SerializeField] ParticleSystem reconstructionParticles;

    [Header("VFX")]
    [SerializeField, Min(0.1f)] float timeVFXCollapse = 0.5f;
    [SerializeField, Min(0.1f)] float timeVFXSpawn = 0.5f;
    [SerializeField, Min(0f)] float collapseSpeed = 1f;

    [Header("SFX")]
    [SerializeField] AudioClip sfxDestruction;
    [SerializeField] AudioClip sfxReconstruction;

    private Collider _collider;
    private Material material;
    private Transform _mesh;
    private AudioSource _audioSource;
    private float _startHealth;
    private Vector3 _spawnPosition;

    public delegate float Score(float score);
    public static event Score OnScore;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
        _startHealth = health;
        _spawnPosition = transform.position;
    }

    private void Start()
    {
        material = GetComponentInChildren<MeshRenderer>().material;
        _mesh = transform.GetChild(0);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == 6)
        {
            health -= 10; // add get damage from player
            if (health <= 0) StartCoroutine(HandleDestructionReconstructionVFX());
        }
    }

    private IEnumerator HandleDestructionReconstructionVFX()
    {
        #region destruction

        OnScore?.Invoke(score);

        if (destructionParticles) destructionParticles.Play();
        if (_audioSource) _audioSource.PlayOneShot(sfxDestruction);

        StartCoroutine(Collapse(collapseSpeed));
        StartCoroutine(FadeInOut(timeVFXCollapse));

        if (_collider) _collider.enabled = false;

        yield return new WaitForSeconds(timeVFXCollapse);

        if (destructionParticles) destructionParticles.Stop();
        if (_mesh) _mesh.gameObject.SetActive(false);

        #endregion

        yield return new WaitForSeconds(timeToBuild);

        #region reconstruction

        _mesh.position = _spawnPosition;

        if (reconstructionParticles) reconstructionParticles.Play();
        if (_audioSource) _audioSource.PlayOneShot(sfxReconstruction);

        if (_collider) _collider.enabled = true;
        if (_mesh) _mesh.gameObject.SetActive(true);

        // Fade In
        yield return StartCoroutine(FadeInOut(timeVFXSpawn));

        if (reconstructionParticles) reconstructionParticles.Stop();

        health = _startHealth;

        #endregion
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
        float colliderSize = gameObject.GetComponent<Collider>().bounds.size.y;

        float duration = colliderSize / speed;
        float startY = _mesh.position.y;
        float targetY = startY - colliderSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float y = Mathf.Lerp(startY, targetY, t);
            _mesh.position = new Vector3(_spawnPosition.x, y, _spawnPosition.z);
            yield return null;
        }
    }
}
