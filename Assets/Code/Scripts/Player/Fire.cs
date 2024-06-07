using UnityEngine;

public class Fire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ParticleSystem fire1;
    [SerializeField] ParticleSystem fire2;

    [Header("Settings")]
    [SerializeField, Min(0.1f)] float fire1Ratio = 0.5f;
    [SerializeField, Range(0, 90)] float coneAngle = 30f;
    [SerializeField, Range(0.1f, 2)] float maxDistance = 0.5f;
    [Space]
    [SerializeField, Min(0.1f)] float fire2Ratio = 0.5f;


    private InputHandler inputHandler;

    private float nextFire1 = 0f;
    private float nextFire2 = 0f;

    public Vector3 test;

    void Start()
    {
        inputHandler = InputHandler.Instance;
    }

    void Update()
    {
        Fire1();
        Fire2();
    }

    void Fire1() // Check
    {
        if (!fire1) return;

        if (inputHandler.fire1Trigger && Time.time > nextFire1)
        {
            nextFire1 = Time.time + fire1Ratio;
            fire1.Play();

            Vector3 coneDirection = transform.forward + Vector3.up;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position + Vector3.up, 0.5f, coneDirection, maxDistance);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer != 6) break;
            }
        }
    }

    void Fire2()
    {
        if (!fire2) return;

        if (inputHandler.fire2Trigger && Time.time > nextFire2)
        {
            nextFire2 = Time.time + fire2Ratio;
            fire2.Play();
        }
    }
}
