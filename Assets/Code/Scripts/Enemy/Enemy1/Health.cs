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
    [SerializeField] ParticleSystem deathVFX;

    private float healthBackup;

    public delegate void Score(float score);
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