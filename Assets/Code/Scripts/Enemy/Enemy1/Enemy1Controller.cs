using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
public class Enemy1Controller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0f)] float fireRatio = 0.5f;
    [SerializeField, Min(0f)] float viweDistance = 10f;
    [SerializeField, Range(0f, 80f)] float viewAngle = 30f;

    [SerializeField] LayerMask targetMask;

    [Header("References")]
    [SerializeField] ParticleSystem fireParticles;
    [Space]
    [SerializeField] Transform turret;
    [SerializeField] Transform barrel;

    //[Header("VFX")]
    //[SerializeField, Min(0.1f)] float timeVFXCollapse = 0.5f;
    //[SerializeField, Min(0.1f)] float timeVFXSpawn = 0.5f;

    [Header("SFX")]
    [SerializeField] AudioClip sfxMove;
    [SerializeField] AudioClip sfxFire;

    private State currentState;
    private Transform target;
    private NavMeshAgent agent;

    private float timeSinceLastFire = 0f;

    private void Start()
    {
        target = Move.Instance.transform;
        agent = GetComponent<NavMeshAgent>();
        currentState = new Idle(this);
    }

    private void Update()
    {
        Debug.Log(currentState.name);
        currentState = currentState.Process();
    }

    // Refernces
    public Transform Target => target;
    public Transform Turret => turret;
    public Transform Barrel => barrel;

    // Settings
    public float FireRatio => fireRatio;
    public float ViewDistance => viweDistance;
    public float ViewAngle => viewAngle;
    public LayerMask TargetMask => targetMask;
    public float TimeSinceLastFire
    {
        get { return timeSinceLastFire; }
        set { timeSinceLastFire = value; }
    }

    // IA
    public NavMeshAgent NavMeshAgent => agent;

    // VFX
    public ParticleSystem FireParticles => fireParticles;

    // SFX
    public AudioClip SfxMove => sfxMove;
    public AudioClip SfxFire => sfxFire;
}