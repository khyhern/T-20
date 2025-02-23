using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hitPoints; // Current health of the enemy
    public string enemyName; // Name of the enemy
    public int damageAmount = 1; // Damage dealt to the player on collision

    public event Action OnDeath; // Event triggered when the enemy dies

    private GameObject dropPrefab; // Prefab of the item to drop
    private int xpAmount; // XP amount associated with the drop
    public float knockbackResistance = 0.5f; // Resistance to knockback forces
    public bool isKnockedBack { get; private set; } // Tracks if the enemy is currently knocked back

    private IEnemyBehavior behavior;

    [Header("Audio Settings")]
    public AudioSource audioSource; // Reference to the AudioSource
    public AudioClip explosionSound; // Sound to play when the enemy explodes


    void Start()
    {
        behavior = GetComponent<IEnemyBehavior>();
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component

        if (behavior == null)
        {
            Debug.LogError("No IEnemyBehavior attached to Enemy!");
            return;
        }

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        behavior.InitializeBehavior(player, rb, 2f); // Example: setting moveSpeed to 2
    }

    void FixedUpdate()
    {
        behavior?.ExecuteBehavior();
    }

    public void Initialize(int hp, string name, GameObject drop, int xp)
    {
        hitPoints = hp;
        enemyName = name;
        dropPrefab = drop;
        xpAmount = xp;

        Debug.Log($"Initialized enemy: {enemyName}, HP: {hitPoints}");
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        Debug.Log($"{enemyName} took {damage} damage. Remaining HP: {hitPoints}");

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback(Vector2 direction, float force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockback = direction.normalized * force * (1 - knockbackResistance);
            rb.AddForce(knockback, ForceMode2D.Impulse);

            isKnockedBack = true;
            Invoke(nameof(EndKnockback), 0.2f);
        }
    }

    private void EndKnockback()
    {
        isKnockedBack = false;
    }

    public void Die()
    {
        Debug.Log($"{enemyName} has died.");

        if (behavior is ExplodingEnemyBehavior explodingBehavior)
        {
            if (audioSource != null && explosionSound != null)
            {
                audioSource.clip = explosionSound;
                audioSource.Play(); // Play explosion sound
                Destroy(gameObject, explosionSound.length); // Destroy after sound finishes
            }
            else
            {
                // No sound available, explode and destroy immediately
                explodingBehavior.TriggerExplosionImmediately();
                Destroy(gameObject);
            }
        }
        else
        {
            // Non-exploding enemies die immediately
            Destroy(gameObject);
        }

        OnDeath?.Invoke();
        DropItem();
    }




    private void DropItem()
    {
        if (dropPrefab == null) return;

        Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Debug.Log($"Dropped {dropPrefab.name} with {xpAmount} XP.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageAmount); // Deal damage to the player
                Debug.Log($"Player took {damageAmount} damage from {enemyName}.");
            }

            // Apply knockback to the player
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerMovement.ApplyKnockback(knockbackDirection, 5f);
            }

            // Apply knockback to the enemy
            Vector2 enemyKnockbackDirection = (transform.position - collision.transform.position).normalized;
            ApplyKnockback(enemyKnockbackDirection, 5f);
        }
    }
}
