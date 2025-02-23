using UnityEngine;

public class WallCollisionHandler : MonoBehaviour
{
    private int damageToPlayer;
    private float knockbackForce;
    private ElectricBarrier parentBarrier;

    public void Setup(int damage, float knockback, ElectricBarrier barrier)
    {
        damageToPlayer = damage;
        knockbackForce = knockback;
        parentBarrier = barrier;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageToPlayer);
                Debug.Log($"Player took {damageToPlayer} damage from the barrier.");
            }

            // Apply knockback to the player
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Vector2 knockbackDirection = CalculateKnockbackDirection(collision.transform.position);
                playerMovement.ApplyKnockback(knockbackDirection, knockbackForce);
                Debug.Log($"Player was knocked back by the barrier in direction {knockbackDirection}.");
            }
        }
    }

    private Vector2 CalculateKnockbackDirection(Vector3 playerPosition)
    {
        // Calculate the relative position of the player to the wall
        Vector3 relativePosition = playerPosition - transform.position;

        // Normalize the direction to make it unit length
        Vector2 knockbackDirection = new Vector2(
            Mathf.Sign(relativePosition.x),
            Mathf.Sign(relativePosition.y)
        ).normalized;

        return knockbackDirection;
    }
}
