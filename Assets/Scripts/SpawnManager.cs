using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [Header("Level Settings")]
    public float levelTime = 1200f; // Total duration of the level in seconds (20 minutes)
    public float darknessModifier = 1.0f; // Multiplier affecting spawn rates (e.g., faster spawns in darker areas)
    public Transform player; // Reference to the player's position in the scene

    [Header("UI")]
    public TMP_Text timerText; // Reference to the TextMeshPro UI component for the timer

    [Header("Spawn Sessions")]
    public List<SpawnSessionData> spawnSessions; // List of spawn configurations for different enemy types

    private Dictionary<string, List<GameObject>> activeEnemies; // Tracks active enemies by type
    private ObjectPoolManager objectPoolManager; // Manages object pooling for efficient spawning

    [Header("Barrier Settings")]
    public GameObject electricBarrierPrefab; // Prefab for the electric barrier

    private bool keyCollected = false;


    void Start()
    {
        // Initialize the dictionary to track active enemies
        activeEnemies = new Dictionary<string, List<GameObject>>();

        // Retrieve the object pool manager component attached to the same GameObject
        objectPoolManager = GetComponent<ObjectPoolManager>();
        if (objectPoolManager == null)
        {
            Debug.LogError("ObjectPoolManager not found! Ensure it is attached to the GameObject.");
            return;
        }

        // Loop through each spawn session configuration
        foreach (var session in spawnSessions)
        {
            // Initialize an empty list to track enemies of this type
            activeEnemies[session.enemyName] = new List<GameObject>();

            // Set up an object pool for this enemy type
            objectPoolManager.InitializePool(session.enemyPrefab, session.maxEnemies);

            // Start handling the spawning process for this session
            StartCoroutine(HandleSpawnSession(session));
        }
    }

    void Update()
    {
        // Only decrement the timer if levelTime is greater than 900
        if (levelTime > 900)
        {
            levelTime -= Time.deltaTime;

            // Ensure it does not drop below 900
            if (levelTime < 900)
            {
                levelTime = 900;
            }

        }

        // Update the timer UI
        UpdateTimerUI();

        // If the level time has run out, trigger the end of the level
        if ((levelTime == 900) && keyCollected)
        {
            EndLevel();
        }
    }

    private void UpdateTimerUI()
    {
        // Clamp levelTime to avoid negative values
        float clampedTime = Mathf.Max(levelTime, 0);

        // Convert time to minutes and seconds
        int minutes = Mathf.FloorToInt(clampedTime / 60);
        int seconds = Mathf.FloorToInt(clampedTime % 60);

        // Update the UI Text with leading zeros for consistency
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    private IEnumerator HandleSpawnSession(SpawnSessionData session)
    {
        // Wait until the level timer reaches the start time of this session
        while (levelTime > session.start)
        {
            yield return null; // Wait for the next frame
        }

        // Spawn enemies while the level time is within the session's active duration
        while (levelTime <= session.start && levelTime > session.end)
        {
            // Wait for the spawn cooldown period, adjusted by the darkness modifier
            float cooldown = Mathf.Max(session.spawnCooldown / Mathf.Max(darknessModifier, 0.1f), 0.1f);
            yield return new WaitForSeconds(cooldown);

            // Check if the maximum number of enemies for this session hasn't been reached
            if (activeEnemies[session.enemyName].Count < session.maxEnemies)
            {
                // Calculate how many enemies to spawn based on the available limit
                int numToSpawn = Mathf.Min(session.numPerSpawn, session.maxEnemies - activeEnemies[session.enemyName].Count);

                // Spawn the calculated number of enemies
                SpawnEnemies(session, numToSpawn);
            }
        }
    }

    private void SpawnEnemies(SpawnSessionData session, int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();

            if (spawnPosition == Vector3.zero)
            {
                Debug.LogWarning("No valid spawn position found. Skipping spawn.");
                continue;
            }

            // Retrieve an enemy from the object pool
            GameObject enemy = objectPoolManager.GetObject(session.enemyPrefab, spawnPosition);

            // Initialize the enemy's properties
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.Initialize(
                    session.hitPoints,
                    session.enemyName,
                    session.dropPrefab,
                    session.xpAmount
                );

                // Attach specific behavior dynamically
                if (session.behaviorScript != null)
                {
                    var behaviorType = session.behaviorScript.GetClass();
                    if (behaviorType != null && typeof(IEnemyBehavior).IsAssignableFrom(behaviorType))
                    {
                        enemy.AddComponent(behaviorType);

                        // Check if this enemy is a boss
                        if (enemy.GetComponent<BossEnemyBehavior>() != null)
                        {
                            CreateBarrierAroundPlayer();
                        }
                    }
                    else
                    {
                        Debug.LogError($"Behavior script {session.behaviorScript.name} does not implement IEnemyBehavior.");
                    }
                }
            }

            activeEnemies[session.enemyName].Add(enemy);
            enemyComponent.OnDeath += () => activeEnemies[session.enemyName].Remove(enemy);
        }
    }

    private void CreateBarrierAroundPlayer()
    {
        if (electricBarrierPrefab == null)
        {
            Debug.LogError("Electric barrier prefab not assigned in SpawnManager!");
            return;
        }

        // Instantiate the barrier around the player
        GameObject barrier = Instantiate(electricBarrierPrefab, player.position, Quaternion.identity);
        ElectricBarrier electricBarrier = barrier.GetComponent<ElectricBarrier>();
        if (electricBarrier != null)
        {
            electricBarrier.ActivateBarrier();
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        // Find the InfiniteMapManager component in the scene
        InfiniteMapManager mapManager = FindObjectOfType<InfiniteMapManager>();
        if (mapManager == null)
        {
            Debug.LogError("InfiniteMapManager not found in the scene!");
            return Vector3.zero; // Return an invalid position if the map manager is missing
        }

        List<Vector3> validPositions = new List<Vector3>();

        // Iterate through all loaded chunks in the map
        foreach (var chunk in mapManager.loadedChunks)
        {
            if (chunk.Value.activeSelf) // Check if the chunk is active
            {
                // Calculate a random position within the bounds of the chunk
                Vector3 chunkCenter = chunk.Value.transform.position;
                Vector3 randomPosition = chunkCenter + new Vector3(
                    Random.Range(-mapManager.chunkSize / 2, mapManager.chunkSize / 2),
                    Random.Range(-mapManager.chunkSize / 2, mapManager.chunkSize / 2),
                    0
                );

                // Add the calculated position to the list of valid spawn positions
                validPositions.Add(randomPosition);
            }
        }

        // Return a random valid position, or Vector3.zero if no valid positions are found
        return validPositions.Count > 0 ? validPositions[Random.Range(0, validPositions.Count)] : Vector3.zero;
    }

    private void EndLevel()
    {
        // Log the end of the level
        Debug.Log("Level ended!");

        SceneManager.LoadScene(2);
    }

    public void OnKeyCollected()
    {
        Debug.Log("Key collected! Spawning event triggered.");
        // Add logic here for what should happen when the key is collected

        keyCollected = true;
    }

}
