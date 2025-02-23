using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapManager : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public int chunkSize = 20; // Size of each chunk in Unity units
    public List<GameObject> chunkPrefabs; // List of possible chunk prefabs
    public int loadDistance = 3; // How many chunks to load around the player

    private Vector2Int currentChunkPos; // The chunk position the player is currently in
    public Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        // Initialize current chunk position and load the initial chunks
        currentChunkPos = GetPlayerChunkPosition();
        LoadChunksAround(currentChunkPos);
    }

    void Update()
    {
        Vector2Int playerChunk = GetPlayerChunkPosition();

        // Check if the player has moved to a new chunk
        if (playerChunk != currentChunkPos)
        {
            currentChunkPos = playerChunk;
            LoadChunksAround(currentChunkPos);

        }
    }

    // Calculate the chunk position based on the player's current position
    Vector2Int GetPlayerChunkPosition()
    {
        return new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.y / chunkSize)
        );
    }

    // Load chunks around the current chunk position
    void LoadChunksAround(Vector2Int center)
    {
        // Activate chunks within the load distance
        for (int x = -loadDistance; x <= loadDistance; x++)
        {
            for (int y = -loadDistance; y <= loadDistance; y++)
            {
                Vector2Int chunkPos = new Vector2Int(center.x + x, center.y + y);
                if (!loadedChunks.ContainsKey(chunkPos))
                {
                    // Create a new chunk if it doesn't exist
                    CreateChunk(chunkPos);
                }
                else if (!loadedChunks[chunkPos].activeSelf)
                {
                    // Reactivate the chunk if it exists but is inactive
                    loadedChunks[chunkPos].SetActive(true);
                }
            }
        }

        // Deactivate chunks that are outside the load distance
        foreach (var chunk in loadedChunks)
        {
            if (Vector2Int.Distance(chunk.Key, center) > loadDistance && chunk.Value.activeSelf)
            {
                chunk.Value.SetActive(false);
            }
        }
    }

    // Create a chunk at the given position
    void CreateChunk(Vector2Int chunkPos)
    {
        Vector3 chunkWorldPos = new Vector3(chunkPos.x * chunkSize, chunkPos.y * chunkSize, 0);
        GameObject selectedChunkPrefab = GetRandomChunkPrefab();
        GameObject newChunk = Instantiate(selectedChunkPrefab, chunkWorldPos, Quaternion.identity);
        loadedChunks[chunkPos] = newChunk;
    }

    // Select a random chunk prefab from the list
    GameObject GetRandomChunkPrefab()
    {
        if (chunkPrefabs.Count == 0)
        {
            Debug.LogError("No chunk prefabs assigned to the Map Manager!");
            return null;
        }
        int randomIndex = Random.Range(0, chunkPrefabs.Count);
        return chunkPrefabs[randomIndex];
    }
}
