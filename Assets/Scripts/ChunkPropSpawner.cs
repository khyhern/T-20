using System.Collections.Generic;
using UnityEngine;

public class ChunkPropSpawner : MonoBehaviour
{
    public List<GameObject> props; // List of props to spawn
    public List<Transform> spawnLocations; // List of spawn points in the chunk

    void Start()
    {
        SpawnProps();
    }

    void SpawnProps()
    {
        if (props.Count == 0 || spawnLocations.Count == 0)
        {
            Debug.LogWarning("No props or spawn locations assigned to the chunk!");
            return;
        }

        foreach (Transform spawnPoint in spawnLocations)
        {
            // Choose a random prop from the list
            GameObject randomProp = props[Random.Range(0, props.Count)];

            // Instantiate the prop at the spawn point
            Instantiate(randomProp, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        }
    }
}
