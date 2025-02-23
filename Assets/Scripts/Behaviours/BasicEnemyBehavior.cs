using UnityEngine;

public class BasicEnemyBehavior : MonoBehaviour, IEnemyBehavior
{
    private Transform player; // Reference to the player's transform
    private Rigidbody2D rb; // Rigidbody2D for physics
    private float moveSpeed; // Speed of movement toward the player
    private Enemy enemyComponent; // Reference to the Enemy component for additional checks

    public void InitializeBehavior(Transform player, Rigidbody2D rb, float moveSpeed)
    {
        this.player = player;
        this.rb = rb;
        this.moveSpeed = moveSpeed;

        enemyComponent = GetComponent<Enemy>();
        if (rb == null)
        {
            Debug.LogError("BasicEnemyBehavior requires a Rigidbody2D component.");
        }

        if (enemyComponent == null)
        {
            Debug.LogError("BasicEnemyBehavior requires an Enemy component.");
        }
    }

    public void ExecuteBehavior()
    {
        if (enemyComponent != null && !enemyComponent.isKnockedBack && player != null)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed; // Move toward the player
    }
}
