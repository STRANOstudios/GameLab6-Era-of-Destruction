using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0f,10f), Tooltip("the curve for the spawn delay")] AnimationCurve spawnDelayCurve;
    [SerializeField, Min(0)] float spawnRadius = 1.0f; // Radius for raycast to check for obstacles
    [SerializeField, Tooltip("the layer on which the object will be not spawned")] LayerMask obstacleLayer;

    [Header("References")]
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Transform defaultSpawnPosition;
    [SerializeField, Tooltip("the surface on which the object will be spawned")] NavMeshSurface navMeshSurface;
    [SerializeField] Camera mainCamera;

    void Start()
    {
        bool isAllSetted = true;

        if (!defaultSpawnPosition)
        {
            Debug.LogWarning("DefaultSpawnPosition not assigned");
            isAllSetted = false;
        }

        if (!objectToSpawn)
        {
            Debug.LogWarning("ObjectToSpawn not assigned");
            isAllSetted = false;
        }

        if (!navMeshSurface)
        {
            Debug.LogWarning("NavMeshSurface not assigned");
            isAllSetted = false;
        }

        if (!mainCamera)
        {
            Debug.LogWarning("MainCamera not assigned");
            isAllSetted = false;
        }

        if (!isAllSetted) return;

        Vector3 spawnPosition = GetValidSpawnPoint();
        if (spawnPosition != Vector3.zero)
        {
            //Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            ObjectPoolerManager.SpawnObject(objectToSpawn, spawnPosition, Quaternion.identity);
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

    bool IsVisibleFrom(Vector3 point, Camera camera)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(point);
        if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0)
        {
            Ray ray = new(camera.transform.position, point - camera.transform.position);
            Debug.DrawRay(camera.transform.position, point - camera.transform.position, Color.cyan, 5.0f); // Draw ray from camera to point
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.position == point)
                {
                    return true;
                }
            }
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
