using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPosition;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks = new List<GameObject>();
    public int maxChunksAllowed = 20; // Prevent excessive chunk spawning
    public float maxOpDist;
    private HashSet<Vector3> existingChunkPositions = new HashSet<Vector3>(); // Prevent duplicates
    private float optimizerCooldown;
    public float optimizerCooldownDur = 1f; // Interval for chunk optimization

    void Start()
    {
        playerLastPosition = player.transform.position;
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string directionName = GetDirectionName(moveDir);
        CheckAndSpawnChunk(directionName);

        // Check additional adjacent directions for diagonal chunks
        if (directionName.Contains("Up")) CheckAndSpawnChunk("Up");
        if (directionName.Contains("Down")) CheckAndSpawnChunk("Down");
        if (directionName.Contains("Right")) CheckAndSpawnChunk("Right");
        if (directionName.Contains("Left")) CheckAndSpawnChunk("Left");
    }

    void CheckAndSpawnChunk(string direction)
    {
        Transform directionTransform = currentChunk.transform.Find(direction);
        if (directionTransform == null) return;

        Vector3 spawnPosition = directionTransform.position;
        
        // Check if chunk already exists before spawning
        if (!existingChunkPositions.Contains(spawnPosition) && 
            !Physics2D.OverlapCircle(spawnPosition, checkerRadius, terrainMask))
        {
            SpawnChunk(spawnPosition);
        }
    }

    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.y > 0.5f) return direction.x > 0 ? "Right Up" : "Left Up";
            else if (direction.y < -0.5f) return direction.x > 0 ? "Right Down" : "Left Down";
            else return direction.x > 0 ? "Right" : "Left";
        }
        else
        {
            if (direction.x > 0.5f) return direction.y > 0 ? "Right Up" : "Right Down";
            else if (direction.x < -0.5f) return direction.y > 0 ? "Left Up" : "Left Down";
            else return direction.y > 0 ? "Up" : "Down";
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        if (spawnedChunks.Count >= maxChunksAllowed)
        {
            Debug.LogWarning("Chunk limit reached, skipping spawn.");
            return;
        }

        int rand = Random.Range(0, terrainChunks.Count);
        GameObject newChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(newChunk);
        existingChunkPositions.Add(spawnPosition); // Track this position
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;
        if (optimizerCooldown > 0f) return;
        optimizerCooldown = optimizerCooldownDur; 

        for (int i = spawnedChunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = spawnedChunks[i];
            float distance = Vector3.Distance(player.transform.position, chunk.transform.position);

            if (distance > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }

        // Remove and destroy extra chunks if limit is exceeded
        while (spawnedChunks.Count > maxChunksAllowed)
        {
            GameObject oldChunk = spawnedChunks[0];
            existingChunkPositions.Remove(oldChunk.transform.position);
            spawnedChunks.RemoveAt(0);
            Destroy(oldChunk);
        }
    }
}
