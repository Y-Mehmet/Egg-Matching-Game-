using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ObjectPool class implements a basic object pooling system using the Singleton pattern.
/// </summary>
public class ObjectPool : Singleton<ObjectPool>
{
    /// <summary>
    /// List of prefab objects that can be instantiated when needed.
    /// </summary>
    public List<GameObject> PrefabsForPool;

    /// <summary>
    /// List of currently pooled (inactive) objects.
    /// </summary>
    List<GameObject> _pooledObjects = new List<GameObject>();

    /// <summary>
    /// Retrieves an object with the specified name from the pool.
    /// </summary>
    /// <param name="objectName">The name of the object to retrieve.</param>
    /// <returns>An active GameObject instance, or null if not found.</returns>
    public GameObject GetObjectFromPool(string objectName)
    {
        // Try to find an inactive object in the pool with the given name
        var instance = _pooledObjects.FirstOrDefault(o => o.name == objectName);

        if (instance != null)
        {
            // Reactivate and return the object
            _pooledObjects.Remove(instance);
            instance.SetActive(true);
            return instance;
        }

        // If no pooled object found, try to instantiate from prefabs
        var prefab = PrefabsForPool.FirstOrDefault(o => o.name == objectName);

        if (prefab != null)
        {
            // Instantiate a new object and return it
            var newInstace = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            newInstace.transform.localPosition = Vector3.zero;
            newInstace.name = objectName;
            return newInstace;
        }

        // If no matching prefab is found, log a warning
        Debug.LogWarning($"{objectName} prefab not found in the prefab list.");
        return null;
    }

    /// <summary>
    /// Adds the specified object back into the pool for future reuse.
    /// </summary>
    /// <param name="gameObject">The GameObject to pool.</param>
    public void PoolObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
        _pooledObjects.Add(gameObject);
    }
}
