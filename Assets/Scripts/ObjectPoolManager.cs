using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool; // Provides access to Unity's Object Pooling API

public class ObjectPoolManager : MonoBehaviour
{
    // Dictionary to store object pools, keyed by the prefab type
    private Dictionary<GameObject, ObjectPool<GameObject>> pools = new Dictionary<GameObject, ObjectPool<GameObject>>();

    /// <summary>
    /// Initializes an object pool for a given prefab.
    /// </summary>
    /// <param name="prefab">The GameObject prefab to pool.</param>
    /// <param name="maxSize">The maximum size of the pool.</param>
    public void InitializePool(GameObject prefab, int maxSize)
    {
        // Check if a pool already exists for this prefab
        if (!pools.ContainsKey(prefab))
        {
            // Create a new object pool for the prefab
            pools[prefab] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefab), // Function to create a new object
                actionOnGet: obj => obj.SetActive(true), // Action to perform when an object is retrieved
                actionOnRelease: obj => obj.SetActive(false), // Action to perform when an object is released
                actionOnDestroy: Destroy, // Action to destroy an object when the pool is cleaned up
                maxSize: maxSize // The maximum number of objects allowed in the pool
            );
        }
    }

    /// <summary>
    /// Retrieves an object from the pool and places it at a specified position.
    /// </summary>
    /// <param name="prefab">The prefab to retrieve from the pool.</param>
    /// <param name="position">The position to place the retrieved object.</param>
    /// <returns>The pooled GameObject.</returns>
    public GameObject GetObject(GameObject prefab, Vector3 position)
    {
        // Check if a pool exists for the given prefab
        if (!pools.ContainsKey(prefab))
        {
            Debug.LogError("Pool not initialized for prefab: " + prefab.name);
            return null; // Return null if no pool is found
        }

        // Retrieve an object from the pool
        GameObject obj = pools[prefab].Get();

        // Set the object's position
        obj.transform.position = position;

        // Return the retrieved object
        return obj;
    }

    /// <summary>
    /// Releases an object back to its pool.
    /// </summary>
    /// <param name="prefab">The prefab type of the object being released.</param>
    /// <param name="obj">The GameObject instance to release.</param>
    public void ReleaseObject(GameObject prefab, GameObject obj)
    {
        // Check if a pool exists for the given prefab
        if (pools.ContainsKey(prefab))
        {
            // Release the object back into the pool
            pools[prefab].Release(obj);
        }
    }
}
