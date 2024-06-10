using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
public class Enemy1Controller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0f)] float fireRatio = 0.5f;
    [SerializeField, Min(0f)] float viewDistance = 10f;
    [SerializeField, Range(0f, 80f)] float viewAngle = 30f;
    [SerializeField, Min(0f)] float safeDistance = 10f;

    [SerializeField] LayerMask targetMask;

    [Header("References")]
    [SerializeField] ParticleSystem fireParticles;
    [Space]
    [SerializeField] Transform body;
    [SerializeField] Transform turret;
    [SerializeField] Transform barrel;

    [Header("VFX")]
    [SerializeField, Min(0.1f)] float rotationAmplitude = 10f;
    //[SerializeField, Min(0.1f)] float timeVFXCollapse = 0.5f;
    //[SerializeField, Min(0.1f)] float timeVFXSpawn = 0.5f;

    [Header("SFX")]
    [SerializeField] AudioClip sfxMove;
    [SerializeField] AudioClip sfxFire;

    private State currentState;
    private Transform target;
    private NavMeshAgent agent;

    private float timeSinceLastFire = 0f;
    private float previusVelocity = 0f;

    private void Start()
    {
        target = Move.Instance.transform;
        agent = GetComponent<NavMeshAgent>();
        currentState = new Idle(this);
    }

    private void Update()
    {
        //Debug.Log(currentState.name);
        currentState = currentState.Process();

        MotionVFX();
    }

    private void MotionVFX()
    {
        float velocity = agent.velocity.magnitude;

        float velocityRound = Mathf.Round(velocity * 100f) / 100f;

        float speed = (velocityRound - previusVelocity) / Time.deltaTime;

        float rotationAngle = 0f; // Inizializza l'angolo di rotazione

        // Imposta l'angolo di rotazione in base alla velocità
        if (speed > 0)
        {
            rotationAngle = -rotationAmplitude;
        }
        else if (speed < 0)
        {
            rotationAngle = rotationAmplitude;
        }
        // Altrimenti, la velocità è 0 e l'angolo di rotazione rimane 0

        previusVelocity = velocityRound;

        Quaternion targetRotation = Quaternion.Euler(rotationAngle, 0f, 0f);

        // Applica la rotazione locale sull'asse delle x al body
        body.localRotation = Quaternion.Lerp(body.localRotation, targetRotation, 5f * Time.deltaTime);
    }


    // Refernces
    public Transform Target => target;
    public Transform Body => body;
    public Transform Turret => turret;
    public Transform Barrel => barrel;

    // Settings
    public float FireRatio => fireRatio;
    public float ViewDistance => viewDistance;
    public float SafeDistance => safeDistance;
    public float ViewAngle => viewAngle;
    public LayerMask TargetMask => targetMask;
    public float TimeSinceLastFire
    {
        get { return timeSinceLastFire; }
        set { timeSinceLastFire = value; }
    }
    public State CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    // IA
    public NavMeshAgent NavMeshAgent => agent;

    // VFX
    public ParticleSystem FireParticles => fireParticles;

    // SFX
    public AudioClip SfxMove => sfxMove;
    public AudioClip SfxFire => sfxFire;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(barrel.position, safeDistance);
    }
}