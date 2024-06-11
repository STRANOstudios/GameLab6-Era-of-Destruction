using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ObjectPoolerManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] float spawnRadius = 1.0f; // Radius for raycast to check for obstacles
    [SerializeField] LayerMask obstacleLayer;

    [Header("References")]
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] Camera mainCamera;

    public static List<PooledObejctInfo> ObjectPools = new();

    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPoint();
            if (spawnPosition != Vector3.zero)
            {
                Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            }
        }
    }

    #region object pooler

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, Transform parent = null)
    {
        PooledObejctInfo pool = ObjectPools.Find(x => x.LoopupString == objectToSpawn.name);

        //if the pool doesn't exist, create it
        if (pool == null)
        {
            pool = new PooledObejctInfo() { LoopupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        //Check if there are any inactive objecsts in the pool
        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            //if there are no inactivate objects, create a new one
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation, parent);
        }
        else
        {
            //if there is an inactive object, reactive it
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name[..^7]; // remove the "(Clone)"
        PooledObejctInfo pool = ObjectPools.Find(x => x.LoopupString == goName);

        if (pool == null)
        {
            Debug.LogWarning("Trying to release an object that is not pooled: " + obj.name);
        }
        else
        {
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }

    #endregion

    #region take position to spawn

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

    bool IsVisibleFrom(Vector3 point, Camera camera) // to be bugfixing
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
            if (hitCollider.gameObject != objectToSpawn)
            {
                return true;
            }
        }
        return false;
    }

    #endregion

}

public class PooledObejctInfo
{
    public string LoopupString;
    public List<GameObject> InactiveObjects = new();
}
