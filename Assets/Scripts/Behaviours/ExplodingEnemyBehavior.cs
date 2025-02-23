using UnityEngine;

public class ExplodingEnemyBehavior : MonoBehaviour, IEnemyBehavior
{
    private Transform player; // Reference to the player's transform
    private Rigidbody2D rb; // Rigidbody2D for physics
    private float moveSpeed; // Speed of movement toward the player
    private Enemy enemyComponent; // Reference to the Enemy component for additional checks

    public float explosionRadius = 3f; // Radius within which the explosion affects other entities
    public int playerDamage = 1; // Damage dealt to the player upon explosion
    public float explosionDelay = 1f; // Delay before the explosion
    private bool isExploding = false; // Tracks if the explosion is already triggered
    public int explosionDamage = 50; // Damage dealt to nearby enemies during the explosion


    public void InitializeBehavior(Transform player, Rigidbody2D rb, float moveSpeed)
    {
        this.player = player;
        this.rb = rb;
        this.moveSpeed = moveSpeed;

        enemyComponent = GetComponent<Enemy>();
        if (rb == null)
        {
            Debug.LogError("ExplodingEnemyBehavior requires a Rigidbody2D component.");
        }

        if (enemyComponent == null)
        {
            Debug.LogError("ExplodingEnemyBehavior requires an Enemy component.");
        }
    }

    public void ExecuteBehavior()
    {
        if (enemyComponent != null && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (!isExploding && distanceToPlayer <= explosionRadius)
            {
                TriggerExplosion();
            }

            // Continue moving toward the player, regardless of explosion status
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed; // Move toward the player
    }

    private void TriggerExplosion()
    {
        if (isExploding) return;

        isExploding = true;
        Debug.Log($"{enemyComponent.enemyName} is about to explode!");
        Invoke(nameof(Explode), explosionDelay);
    }

    public void TriggerExplosionImmediately()
    {
        if (isExploding) return;

        isExploding = true;
        Debug.Log($"{enemyComponent.enemyName} is exploding immediately!");
        Explode();
    }

    private void Explode()
    {
        if (enemyComponent == null) return;

        // Damage the player if within range
        if (Vector2.Distance(transform.position, player.position) <= explosionRadius)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(playerDamage);
            }
        }

        // Apply damage to all enemies within the explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            Enemy nearbyEnemy = hit.GetComponent<Enemy>();
            if (nearbyEnemy != null && nearbyEnemy != enemyComponent)
            {
                nearbyEnemy.TakeDamage(explosionDamage); // Apply 50 damage to the enemy
            }
        }

        // Destroy this enemy after exploding
        Debug.Log($"{enemyComponent.enemyName} exploded!");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the explosion radius in the scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
