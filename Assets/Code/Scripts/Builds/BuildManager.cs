using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Collider))]
public class BuildManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0.1f)] float health = 100f;

    [SerializeField, Range(0f, 20f)] float timeToBuild = 5f;

    [Header("References")]
    [SerializeField] ParticleSystem destructionParticles;
    [SerializeField] ParticleSystem reconstructionParticles;

    [Header("VFX")]
    [SerializeField, Min(0.1f)] float timeVFXCollapse = 0.5f;
    [SerializeField, Min(0.1f)] float timeVFXSpawn = 0.5f;

    private Collider _collider;
    private Material material;
    private Transform _mesh;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
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
            health -= 10;
            if (health <= 0)
            {
                StartCoroutine(HandleDestructionReconstructionVFX());
            }
        }
    }

    private IEnumerator HandleDestructionReconstructionVFX()
    {
        // Distruzione
        if (destructionParticles != null)
        {
            destructionParticles.Play();
        }

        // Effetto di dissolvenza
        yield return StartCoroutine(FadeInOut(timeVFXCollapse));

        yield return new WaitForSeconds(timeVFXCollapse);

        if (destructionParticles != null)
        {
            destructionParticles.Stop();
        }
        if (_collider != null)
        {
            _mesh.gameObject.SetActive(false);
            _collider.enabled = false;
        }

        // Ricostruzione
        yield return new WaitForSeconds(timeToBuild);

        if (reconstructionParticles != null)
        {
            reconstructionParticles.Play();
        }

        if (_collider != null)
        {
            _mesh.gameObject.SetActive(true);
            _collider.enabled = true;
        }

        // Effetto di apparizione
        yield return StartCoroutine(FadeInOut(timeVFXSpawn));

        if (reconstructionParticles != null)
        {
            reconstructionParticles.Stop();
        }
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
}
