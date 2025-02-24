using UnityEngine;

public class SpawnEnemyOnDeath : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    public GameObject enemyPrefab; // The enemy to spawn upon death
    public int spawnCount = 2; // Number of enemies to spawn
    public Vector2 spawnOffset = new Vector2(1f, 1f); // Offset range for spawned enemies

    [Header("New Enemy Stats")]
    public int newHealth = 10;
    public float newSpeed = 2f;
    public int newDamage = 5;

    private bool isQuitting = false;

    void OnApplicationQuit()
    {
        isQuitting = true; // Prevents spawning when quitting the game
    }

    private void OnDestroy()
    {
        if (isQuitting || !gameObject.scene.isLoaded) return; // Prevents errors during scene unload

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null) return;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomOffset = new Vector2(
                Random.Range(-spawnOffset.x, spawnOffset.x),
                Random.Range(-spawnOffset.y, spawnOffset.y)
            );

            Vector2 spawnPosition = (Vector2)transform.position + randomOffset;
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // Set the new enemy's stats
            EnemyAI newEnemyAI = newEnemy.GetComponent<EnemyAI>();
            if (newEnemyAI != null)
            {
                newEnemyAI.healthPoints = newHealth;
                newEnemyAI.speed = newSpeed;
                newEnemyAI.damageToPlayer = newDamage;
            }
        }
    }
}
