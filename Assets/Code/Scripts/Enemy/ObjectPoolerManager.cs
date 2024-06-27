using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolerManager : MonoBehaviour
{
    public static List<PooledObejctInfo> ObjectPools = new();

    /// <summary>
    /// Spawns an object from the pool
    /// </summary>
    /// <param name="objectToSpawn"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="spawnRotation"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
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

        if (!spawnableObj)
        {
            //if there are no inactivate objects, create a new one
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation, parent);
        }
        else
        {
            //if there is an inactive object, reactive it
            spawnableObj.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    /// <summary>
    /// Return an object to the pool
    /// </summary>
    /// <param name="obj"></param>
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
}

public class PooledObejctInfo
{
    public string LoopupString;
    public List<GameObject> InactiveObjects = new();
}
