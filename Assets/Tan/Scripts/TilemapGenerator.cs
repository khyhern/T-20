using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    public GameObject[] tilemapPrefabs; // Array of different tilemaps to spawn
    public Transform player;            // Player reference for tracking movement
    public int chunkWidth = 30;         // Width of each chunk (in tiles)
    public int bufferSize = 30;          // How close the player needs to be before generating new chunks
    public int maxActiveChunks = 3;     // Max chunks to keep active at any time
    public float spawnAheadDistance = 10f; // Preload chunks ahead of the player

    private float tilemapWidthWorld; // World width of a single chunk
    private Dictionary<int, GameObject> activeChunks = new Dictionary<int, GameObject>();
    private int leftmostChunk = 0;   // Tracks the leftmost generated chunk
    private int rightmostChunk = 0;  // Tracks the rightmost generated chunk

    void Start()
    {
        if (tilemapPrefabs.Length == 0)
        {
            Debug.LogError("No tilemap prefabs assigned!");
            return;
        }

        // Calculate chunk width in world units
        tilemapWidthWorld = chunkWidth * GetTileSize();

        // Spawn initial chunks ahead of the player
        int startChunkIndex = Mathf.FloorToInt(player.position.x / tilemapWidthWorld);
        for (int i = -1; i <= 1; i++)
        {
            InstantiateChunk(Vector3.right * (startChunkIndex + i) * tilemapWidthWorld, startChunkIndex + i);
        }
    }

    void Update()
    {
        if (player == null) return;

        float playerX = player.position.x;

        // Generate chunks earlier to match the camera position
        float rightSpawnThreshold = (rightmostChunk + 1) * tilemapWidthWorld - bufferSize - spawnAheadDistance;
        float leftSpawnThreshold = leftmostChunk * tilemapWidthWorld + bufferSize + spawnAheadDistance;

        if (playerX > rightSpawnThreshold)
        {
            InstantiateChunk(Vector3.right * (rightmostChunk + 1) * tilemapWidthWorld, rightmostChunk + 1);
        }
        if (playerX < leftSpawnThreshold)
        {
            InstantiateChunk(Vector3.right * (leftmostChunk - 1) * tilemapWidthWorld, leftmostChunk - 1);
        }

        // Remove old chunks
        DespawnOutOfRangeChunks(playerX);
    }

    void InstantiateChunk(Vector3 position, int chunkIndex)
    {
        // Skip if chunk is already active
        if (activeChunks.ContainsKey(chunkIndex)) return;

        // Pick a random tilemap from the array
        GameObject chosenPrefab = tilemapPrefabs[Random.Range(0, tilemapPrefabs.Length)];

        // Instantiate the chunk
        GameObject newChunk = Instantiate(chosenPrefab, position, Quaternion.identity, transform);
        activeChunks[chunkIndex] = newChunk;

        // Update bounds
        if (chunkIndex > rightmostChunk) rightmostChunk = chunkIndex;
        if (chunkIndex < leftmostChunk) leftmostChunk = chunkIndex;
    }

    void DespawnOutOfRangeChunks(float playerX)
    {
        float leftBound = playerX - (maxActiveChunks * tilemapWidthWorld);
        float rightBound = playerX + (maxActiveChunks * tilemapWidthWorld);

        List<int> chunksToRemove = new List<int>();

        foreach (var chunk in activeChunks)
        {
            float chunkCenter = chunk.Key * tilemapWidthWorld;
            if (chunkCenter < leftBound || chunkCenter > rightBound)
            {
                chunksToRemove.Add(chunk.Key);
            }
        }

        foreach (int chunkIndex in chunksToRemove)
        {
            Destroy(activeChunks[chunkIndex]);
            activeChunks.Remove(chunkIndex);
        }
    }

    float GetTileSize()
    {
        // Get the tile size from the first prefab
        if (tilemapPrefabs.Length > 0)
        {
            var grid = tilemapPrefabs[0].GetComponentInParent<Grid>();
            return grid != null ? grid.cellSize.x : 1.0f;
        }
        return 1.0f;
    }
}



