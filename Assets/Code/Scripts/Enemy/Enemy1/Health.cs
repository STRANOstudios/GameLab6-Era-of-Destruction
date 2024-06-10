using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] float health = 100f;
    [SerializeField, Min(0)] float score = 100f;

    [Header("References")]
    [SerializeField] ParticleSystem deathVFX;

    private float healthBackup;

    public delegate float Score(float score);
    public static event Score OnScore;

    private void Awake()
    {
        healthBackup = health;
    }

    private void OnEnable()
    {
        health = healthBackup;
        GetComponent<Collider>().enabled = true;
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
        OnScore?.Invoke(score);
        if (deathVFX) deathVFX.Play();

        GetComponent<Enemy1Controller>().CurrentState = new Death(GetComponent<Enemy1Controller>());
        GetComponent<Collider>().enabled = false;

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