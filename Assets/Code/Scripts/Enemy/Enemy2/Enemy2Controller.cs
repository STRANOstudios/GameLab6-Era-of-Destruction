using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Enemy2Controller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0f)] float speed = 1f;
    [SerializeField, Min(0f)] float impactDelay = 1f;
    [SerializeField, Min(0f)] float fireRatio = 1f;

    [Header("Refereces")]
    [SerializeField] List<MissileLauncher> missileLauncher = new();

    private Transform target;
    private float timeSinceLastFire = 0f;
    private bool isTargetting = false;

    private void Start()
    {
        target = Move.Instance.transform;
    }

    private void Update()
    {
        if (!isTargetting)
        {
            transform.position = Vector3.MoveTowards(transform.position, new(target.position.x, transform.position.y, target.position.z), speed * Time.deltaTime);
        }

        if (Time.time - timeSinceLastFire > fireRatio + impactDelay)
        {
            StartCoroutine(Shoot());

            timeSinceLastFire = Time.time;

            foreach (var missile in missileLauncher)
            {
                missile.Shoot(impactDelay);
            }
        }
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSecondsRealtime(impactDelay / 3f);

        isTargetting = true;

        yield return new WaitForSecondsRealtime(impactDelay);

        isTargetting = false;
    }
}
