using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] float health = 100f;
    [SerializeField, Min(0)] int score = 100;
    [SerializeField] LayerMask layerMask;

    [Header("References")]
    [SerializeField] GameObject defaultMesh;
    [SerializeField] GameObject rubbleMesh;

    [Header("VFX")]
    [SerializeField, Tooltip("VFX explosion")] ParticleSystem deathVFX;

    [Header("SFX")]
    [SerializeField, Tooltip("SFX explosion")] AudioClip explosionSFX;

    private float healthBackup;

    private AudioSource audioSource;

    public delegate void Score(float score);
    public static event Score OnScore;

#if UNITY_EDITOR

    private void OnValidate()
    {
        // VFX
        if (!deathVFX) Debug.LogWarning("deathVFX not assigned");
        if (!defaultMesh) Debug.LogWarning("defaultMesh not assigned");
        if (!rubbleMesh) Debug.LogWarning("rubbleMesh not assigned");

        // SFX
        if (!explosionSFX) Debug.LogWarning("explosion not assigned");
    }

#endif

    private void Awake()
    {
        healthBackup = health;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        health = healthBackup;
        GetComponent<Collider>().enabled = true;
        ChangeMesh(true);
    }

    private void OnParticleCollision(GameObject other)
    {
        if ((layerMask.value & (1 << other.layer)) != 0)
        {
            TakeDamage(other.GetComponent<Damage>().GetDamage);
        }
    }

    /// <summary>
    /// Take damage.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        OnScore?.Invoke((float)score);
        if (deathVFX) deathVFX.Play();
        if (audioSource && explosionSFX) audioSource.PlayOneShot(explosionSFX);

        ChangeMesh(false);

        GetComponent<Enemy1Controller>().CurrentState = new Death(GetComponent<Enemy1Controller>());
        GetComponent<Collider>().enabled = false;

        yield return new WaitUntil(() => !deathVFX.isPlaying);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Change the mesh
    /// </summary>
    /// <param name="value"></param>
    private void ChangeMesh(bool value)
    {
        if (defaultMesh) defaultMesh.SetActive(value);
        if (rubbleMesh) rubbleMesh.SetActive(!value);
    }
}

//[CustomEditor(typeof(Health))]
//public class MyScriptEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        Health myScript = (Health)target;
//        if (GUILayout.Button("Esegui MyMethod"))
//        {
//            myScript.TakeDamage(10);
//        }
//    }
//}