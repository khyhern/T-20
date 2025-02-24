using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerProgress : MonoBehaviour
{
    public Text tilesRemainingText; // UI text to display tiles left
    public Transform playerTransform; // Player's transform
    public float tileSize = 1f; // Size of one tile
    public int requiredTiles = 800; // Tiles needed to reach the next level

    private float startX; // Initial position
    private int tilesMoved = 0;

    [Header("Scene References")]
    public string nextLevelScene; // Set the next level scene in Inspector
    public string loseScene; // Set the lose scene in Inspector

    void Start()
    {
        startX = playerTransform.position.x;
        UpdateTilesRemainingUI();
    }

    void Update()
    {
        // Calculate how many tiles the player has moved
        int currentTilesMoved = Mathf.FloorToInt((playerTransform.position.x - startX) / tileSize);

        if (currentTilesMoved > tilesMoved) // Update only when moving forward
        {
            tilesMoved = currentTilesMoved;
            UpdateTilesRemainingUI();
        }

        // Check if player reached the goal
        if (tilesMoved >= requiredTiles)
        {
            AdvanceToNextLevel();
        }
    }

    void UpdateTilesRemainingUI()
    {
        int tilesLeft = Mathf.Max(0, requiredTiles - tilesMoved);
        tilesRemainingText.text = "Objective: " + tilesLeft + " m";
    }

    void AdvanceToNextLevel()
    {
        Debug.Log("Level Complete! Loading next level...");
        
        if (nextLevelScene != null)
        {
            SceneManager.LoadScene(nextLevelScene); // Change to your next scene name
        }
        else
        {
            Debug.Log("Set a win scene for level 2");
        }
    }

    public void TimeUp()
    {
        Debug.Log("Time's up! You lost.");
        
        if (loseScene != null)
        {
            SceneManager.LoadScene(loseScene); // Change to your lose scene name
        }
        else
        {
            Debug.Log("Set a lose scene for level 2");
        }
    }
}
