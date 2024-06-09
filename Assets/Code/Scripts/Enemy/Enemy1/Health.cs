using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] float health = 100f;

    [Header("References")]
    [SerializeField] ParticleSystem deathVFX;

    private float healthBackup;

    private void Awake()
    {
        healthBackup = health;
    }

    private void OnEnable()
    {
        health = healthBackup;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == 6)
        {
            TakeDamage(10f);
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
        if (deathVFX) deathVFX.Play();
        yield return new WaitUntil(() => !deathVFX.isPlaying);
        gameObject.SetActive(false);
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