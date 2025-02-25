using UnityEngine;

public class XPOrbController : MonoBehaviour
{
    public float followDistance = 5f;      // Distance within which the player should be followed
    public float moveSpeed = 3f;           // Speed at which the object follows the player

    public GameObject trailEffect;         // Reference to the trail effect prefab (set in inspector)

    private GameObject player;             // Reference to the player object (tagged as 'Player')

    public int expAmount = 100;


    void Start()
    {
        // Find the player object by its tag when the script starts
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No player found!");
        }

        // Optionally instantiate the trail effect if needed
        if (trailEffect != null)
        {
            Instantiate(trailEffect, transform.position, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Check if the player is within range
            if (Vector2.Distance(transform.position, player.transform.position) <= followDistance)
            {
                // Move the object towards the player at the specified moveSpeed
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            // Destroy the orb when it comes in contact with the player
            Debug.Log("XP Orb collected by player.");
            Destroy(gameObject);
            ExperienceManager.Instance.AddExperience(expAmount);
        }        
    }
}



/*
{
    public float followDistance = 5f;      // Distance within which the player should be followed
    public float moveSpeed = 3f;           // Speed at which the object follows the player
    private GameObject player;             // Reference to the player object (tagged as 'Player')

    void Start()
    {
        // Find the player object by its tag when the script starts
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No player found!");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Check if the player is within range
            if (Vector2.Distance(transform.position, player.transform.position) <= followDistance)
            {
                // Move the object towards the player at the specified moveSpeed
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Destroy the object when it comes in contact with the player
            Destroy(gameObject);
        }
    }
}
*/