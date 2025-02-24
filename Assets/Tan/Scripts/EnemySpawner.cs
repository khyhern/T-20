using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPrefabStats
{
    public GameObject enemyPrefab;
    public float speed = 3f;
    public int healthPoints = 10;
    public int damageToPlayer = 1;
    public float knockbackForce = 5f;
    public float spawnRate = 2f; // Individual spawn rate
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private bool canSpawn = true;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<EnemyPrefabStats> enemyList = new List<EnemyPrefabStats>();

    private Dictionary<GameObject, float> enemySpawnRates = new Dictionary<GameObject, float>(); // Store individual spawn rates
    private Dictionary<GameObject, float> nextSpawnIncreaseTimes = new Dictionary<GameObject, float>(); // Track when to increase spawn rates

    private void Start()
    {
        // Initialize spawn rates per enemy type
        foreach (var enemy in enemyList)
        {
            enemySpawnRates[enemy.enemyPrefab] = enemy.spawnRate;
            nextSpawnIncreaseTimes[enemy.enemyPrefab] = FindAnyObjectByType<CountdownTimer>().timeRemaining - 90f; // Start checking after 1 min 30 sec

            StartCoroutine(Spawner(enemy)); // Start individual spawn coroutine
        }

        CountdownTimer.OnDifficultyIncrease += HandleDifficultyIncrease; // Subscribe to event
    }

    private void OnDestroy()
    {
        CountdownTimer.OnDifficultyIncrease -= HandleDifficultyIncrease; // Unsubscribe to prevent memory leaks
    }

    private IEnumerator Spawner(EnemyPrefabStats enemy)
    {
        while (canSpawn)
        {
            yield return new WaitForSeconds(enemySpawnRates[enemy.enemyPrefab]);

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject spawnedEnemy = Instantiate(enemy.enemyPrefab, spawnPoint.position, Quaternion.identity);

            EnemyAI enemyAI = spawnedEnemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.speed = enemy.speed;
                enemyAI.healthPoints = enemy.healthPoints;
                enemyAI.damageToPlayer = enemy.damageToPlayer;
                enemyAI.knockbackForce = enemy.knockbackForce;
            }

            Debug.Log($"Spawned {enemy.enemyPrefab.name} with {enemy.healthPoints} HP");
        }
    }

    private void HandleDifficultyIncrease()
    {
        float timeRemaining = FindAnyObjectByType<CountdownTimer>().timeRemaining;

        // Increase enemy health
        IncreaseEnemyHealth();

        // Every 90 seconds, check for each enemy type and increase its spawn rate
        foreach (var enemy in enemyList)
        {
            if (timeRemaining <= nextSpawnIncreaseTimes[enemy.enemyPrefab])
            {
                IncreaseSpawnRate(enemy);
                nextSpawnIncreaseTimes[enemy.enemyPrefab] -= 90f; // Set next spawn increase interval
            }
        }
    }

    private void IncreaseEnemyHealth()
    {
        foreach (var enemy in enemyList)
        {
            enemy.healthPoints += 20; // Add +20 health
            Debug.Log($"{enemy.enemyPrefab.name} health increased to {enemy.healthPoints}");
        }
    }

    private void IncreaseSpawnRate(EnemyPrefabStats enemy)
    {
        enemySpawnRates[enemy.enemyPrefab] = Mathf.Max(0.5f, enemySpawnRates[enemy.enemyPrefab] - 1f); // Reduce spawn time
        Debug.Log($"Spawn rate for {enemy.enemyPrefab.name} increased! New spawn rate: {enemySpawnRates[enemy.enemyPrefab]} seconds.");
    }
}







/*{
    [SerializeField] private float spawnRate = 1f; // Rate of spawning enemies

    [SerializeField] private GameObject[] enemyPrefabs; // Array of enemy prefabs to spawn

    [SerializeField] private bool canSpawn = true; // Bool to control spawning

    [SerializeField] private Transform[] spawnPoints; // Array of spawn points

    private void Start()
    {
        StartCoroutine(Spawner()); // Start the spawn coroutine
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate); // Delay between spawns

        while (canSpawn)
        {
            yield return wait;

            // Randomly select an enemy prefab to spawn
            int randEnemy = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[randEnemy];

            // Randomly select a spawn point
            int randSpawnPoint = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randSpawnPoint];

            // Instantiate the selected enemy at the randomly selected spawn point
            Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);
        }
    }
}*/
