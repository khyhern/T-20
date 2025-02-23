using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10; // Damage dealt by the bullet
    public float lifespan = 5f; // Lifespan of the bullet in seconds

    private ObjectPoolManager poolManager; // Reference to the object pool manager
    private Rigidbody2D rb; // Reference to Rigidbody2D for physics
    private GameObject prefabReference; // Reference to the bullet prefab for pooling
    private float timeSinceSpawned; // Tracks time since the bullet was spawned

    private bool isReleased = false; // Flag to check if the bullet is already released

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (poolManager == null)
        {
            poolManager = FindObjectOfType<ObjectPoolManager>();
        }
    }

    private void OnEnable()
    {
        timeSinceSpawned = 0f; // Reset the timer when the bullet is spawned
        isReleased = false;    // Reset the release flag
    }

    private void Update()
    {
        // Track the bullet's lifespan
        timeSinceSpawned += Time.deltaTime;
        if (timeSinceSpawned >= lifespan)
        {
            ReturnToPoolOrDestroy();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the bullet hit an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Deal damage to the enemy
                enemy.TakeDamage(damage);
            }
        }

        // Return the bullet to the pool or destroy it
        ReturnToPoolOrDestroy();
    }

    private void ReturnToPoolOrDestroy()
    {
        if (isReleased) return; // Prevent double release

        isReleased = true; // Mark as released

        if (poolManager != null && prefabReference != null)
        {
            // Return the bullet to the pool
            poolManager.ReleaseObject(prefabReference, gameObject);
        }
        else
        {
            // If pooling is not set up, destroy the bullet
            Debug.LogWarning("PoolManager or PrefabReference is null, destroying bullet.");
            Destroy(gameObject);
        }
    }

    public void SetPrefabReference(GameObject prefab)
    {
        prefabReference = prefab; // Store the prefab reference
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity; // Set bullet velocity
    }
}
