using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Tooltip("the curve for the spawn delay")] AnimationCurve spawnDelayCurve;
    [SerializeField, Min(0), Tooltip("the duration of the gameplay")] float gameplayDuration = 1.0f;
    [SerializeField, Min(0), Tooltip("the spawn interval in seconds")] float spawnInterval = 1.0f;
    [SerializeField, Min(0)] float spawnRadius = 1.0f; // Radius for raycast to check for obstacles
    [SerializeField, Tooltip("the layer on which the object will be not spawned")] LayerMask obstacleLayer;
    [SerializeField, Min(0), Tooltip("the tolerance for the raycast")] float tolerance = 0.5f;

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
        if (Time.time > spawnDelay)
        {
            spawnDelay = Time.time + spawnInterval * spawnDelayCurve.Evaluate(Time.time / gameplayDuration);
            Spawn();
        }
    }

    void Spawn(int amount = 1)
    {
        for (int attempt = 0; attempt < amount; attempt++)
        {
            Vector3 spawnPosition = GetValidSpawnPoint();
            if (spawnPosition != Vector3.zero)
            {
                //Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                ObjectPoolerManager.SpawnObject(objectToSpawn, spawnPosition, Quaternion.identity);
            }
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
        Vector3 randomPoint = new(
            Random.Range(navMeshSurface.transform.position.x - navMeshSurface.size.x / 2, navMeshSurface.transform.position.x + navMeshSurface.size.x / 2),
            navMeshSurface.transform.position.y,
            Random.Range(navMeshSurface.transform.position.z - navMeshSurface.size.z / 2, navMeshSurface.transform.position.z + navMeshSurface.size.z / 2)
        );

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
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
        Collider[] hitColliders = Physics.OverlapSphere(point, spawnRadius, obstacleLayer);
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
}
