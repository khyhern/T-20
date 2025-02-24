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
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private bool canSpawn = true;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<EnemyPrefabStats> enemyList = new List<EnemyPrefabStats>();

    private void Start()
    {
        StartCoroutine(Spawner());
        CountdownTimer.OnDifficultyIncrease += IncreaseEnemyHealth; // Subscribe to event
    }

    private void OnDestroy()
    {
        CountdownTimer.OnDifficultyIncrease -= IncreaseEnemyHealth; // Unsubscribe to prevent memory leaks
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (canSpawn)
        {
            yield return wait;

            int randIndex = Random.Range(0, enemyList.Count);
            EnemyPrefabStats selectedEnemy = enemyList[randIndex];

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject spawnedEnemy = Instantiate(selectedEnemy.enemyPrefab, spawnPoint.position, Quaternion.identity);

            EnemyAI enemyAI = spawnedEnemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.speed = selectedEnemy.speed;
                enemyAI.healthPoints = selectedEnemy.healthPoints;
                enemyAI.damageToPlayer = selectedEnemy.damageToPlayer;
                enemyAI.knockbackForce = selectedEnemy.knockbackForce;
            }

            Debug.Log($"Spawned {selectedEnemy.enemyPrefab.name} with {selectedEnemy.healthPoints} HP");
        }
    }

    private void IncreaseEnemyHealth()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].healthPoints += 20; // Add +20 health
            Debug.Log($"{enemyList[i].enemyPrefab.name} health increased to {enemyList[i].healthPoints}");
        }
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
