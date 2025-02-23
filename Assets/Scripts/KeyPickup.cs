using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public SpawnManager spawnManager; // Reference to SpawnManager

    private void Start()
    {
        if (spawnManager == null)
        {
            spawnManager = FindObjectOfType<SpawnManager>(); // Auto-find SpawnManager in the scene
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure only the player can pick it up
        {
            if (spawnManager != null)
            {
                spawnManager.OnKeyCollected(); // Notify the SpawnManager
            }

            Destroy(gameObject); // Remove key after pickup
        }
    }
}