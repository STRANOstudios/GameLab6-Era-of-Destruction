using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0), Tooltip("the start delay in seconds")] float startDelay = 0.0f;
    [SerializeField, Tooltip("the curve for the spawn delay")] AnimationCurve spawnDelayCurve;
    [SerializeField, Min(0), Tooltip("the duration of the gameplay")] float gameplayDuration = 1.0f;
    [SerializeField, Min(0), Tooltip("the spawn interval in seconds")] float spawnInterval = 1.0f;
    [Space]
    [SerializeField, Min(0)] float spawnRadius = 1.0f; // Radius for raycast to check for obstacles
    [SerializeField, Min(0), Tooltip("the tolerance for the raycast")] float tolerance = 0.5f;
    [SerializeField, Min(0), Tooltip("the area for the raycast")] float spawnArea = 10;

    [Header("References")]
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Transform defaultSpawnPosition;
    [SerializeField, Tooltip("the surface on which the object will be spawned")] NavMeshSurface navMeshSurface;
    [SerializeField] Camera mainCamera;

    private float spawnDelay = 0.0f;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!defaultSpawnPosition) Debug.LogWarning("DefaultSpawnPosition not assigned");
        if (!objectToSpawn) Debug.LogWarning("ObjectToSpawn not assigned");
        if (!navMeshSurface) Debug.LogWarning("NavMeshSurface not assigned");
        if (!mainCamera) Debug.LogWarning("MainCamera not assigned");
    }

#endif

    void Update()
    {
        if (Time.time > spawnDelay && Time.time > startDelay)
        {
            spawnDelay = Time.time + spawnInterval * spawnDelayCurve.Evaluate(Time.time / gameplayDuration);
            Spawn();
        }
    }

    void Spawn(int amount = 1)
    {
        for (int attempt = 0; attempt < amount; attempt++)
        {
            ObjectPoolerManager.SpawnObject(objectToSpawn, GetValidSpawnPoint(), Quaternion.identity);
        }
    }

    Vector3 GetValidSpawnPoint()
    {
        for (int attempt = 0; attempt < 100; attempt++) // Try 100 times to find a valid point
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh();

            if (randomPoint != Vector3.zero && !IsVisibleFrom(randomPoint, mainCamera) && !IsObstructed(randomPoint))
            {
                return randomPoint;
            }
        }
        return Vector3.zero; // Return zero vector if no valid point is found
    }

    Vector3 GetRandomPointOnNavMesh()
    {
        Vector3 randomPoint = Random.insideUnitSphere * spawnArea;
        randomPoint += navMeshSurface.transform.position;

        Debug.DrawRay(randomPoint, Vector3.up, Color.red, 5.0f);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, tolerance, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }

    bool IsVisibleFrom(Vector3 point, Camera camera) // to be bugfixed
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(point);

        if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0)
        {
            Ray ray = new(camera.transform.position, point - camera.transform.position);
            RaycastHit[] hits = Physics.RaycastAll(ray, (point - camera.transform.position).magnitude);

            Debug.DrawRay(camera.transform.position, point - camera.transform.position, Color.cyan, 5.0f); // Draw ray from camera to point

            return !(hits.Length > 0);
        }

        return false;
    }

    bool IsObstructed(Vector3 point)
    {
        Collider[] hitColliders = Physics.OverlapSphere(point, spawnRadius, navMeshSurface.gameObject.layer);

        Debug.DrawRay(point, Vector3.up * 2, Color.green, 5.0f); // Draw ray at the spawn position

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == objectToSpawn)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(navMeshSurface.transform.position, spawnArea);
    }
}
