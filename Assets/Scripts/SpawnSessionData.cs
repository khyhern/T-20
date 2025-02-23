using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/SpawnSessionData")]
public class SpawnSessionData : ScriptableObject
{
    public string enemyName; // Name of the enemy
    public GameObject enemyPrefab; // Prefab of the enemy
    public float start; // Start time of the session (in seconds)
    public float end; // End time of the session (in seconds)
    public int hitPoints; // HP of enemies in this session
    public int maxEnemies; // Max number of enemies at a time (-1 for unlimited)
    public int numPerSpawn; // Number of enemies per spawn
    public float spawnCooldown; // Time in seconds between spawns

    [Header("Behavior Settings")]
    public MonoScript behaviorScript; // Reference to the behavior script file

    [Header("Drop Settings")]
    public GameObject dropPrefab; // Item prefab to drop upon death
    public int xpAmount; // XP amount for the drop
}
