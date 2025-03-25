using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Game/SpawnSessionData")]
public class SpawnSessionData : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    public float start;
    public float end;
    public int hitPoints;
    public int maxEnemies;
    public int numPerSpawn;
    public float spawnCooldown;

    [Header("Behavior Settings")]

    // Runtime-safe fallback
    public string behaviorScriptName;

    // Editor-only field
#if UNITY_EDITOR
    public MonoScript behaviorScript;
#endif

    [Header("Drop Settings")]
    public GameObject dropPrefab;
    public int xpAmount;

#if UNITY_EDITOR
    // This runs in the editor and syncs the MonoScript class name to the string
    private void OnValidate()
    {
        if (behaviorScript != null)
        {
            behaviorScriptName = behaviorScript.GetClass().FullName;
        }
    }
#endif
}
