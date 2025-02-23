using UnityEngine;

public class BossEnemyBehavior : MonoBehaviour, IEnemyBehavior
{
    private Transform player; // Reference to the player's transform
    private Rigidbody2D rb; // Rigidbody2D for physics

    private Vector2 dashDirection; // Direction locked during the charge-up phase
    private bool isCharging = false; // Indicates if the boss is charging up for a dash
    private bool isDashing = false; // Indicates if the boss is currently dashing
    private bool isSlowingDown = false; // Indicates if the boss is slowing down

    public float chaseSpeed = 3f; // Speed while chasing the player
    public float dashSpeed = 10f; // Dash speed
    public float chargeTime = 1f; // Time to charge before dashing
    public float dashRange = 5f; // Distance the boss dashes
    public float slowDownDuration = 1f; // Time to slow down after a dash
    public float stopDuration = 1f; // Time to stop completely before resuming behavior

    public void InitializeBehavior(Transform player, Rigidbody2D rb, float moveSpeed)
    {
        this.player = player;
        this.rb = rb;
        this.chaseSpeed = moveSpeed;

        if (rb == null)
        {
            Debug.LogError("BossEnemyBehavior requires a Rigidbody2D component.");
        }
    }

    public void ExecuteBehavior()
    {
        if (player == null || isCharging || isDashing || isSlowingDown)
        {
            // Skip behavior if charging, dashing, or slowing down
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > dashRange)
        {
            ChasePlayer(); // Chase the player if outside dashing range
        }
        else
        {
            StartCharge(); // Start charging if within dashing range
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed; // Chase the player
        Debug.Log("Boss is chasing the player.");
    }

    private void StartCharge()
    {
        if (isCharging || isDashing) return;

        isCharging = true;
        dashDirection = (player.position - transform.position).normalized; // Lock dash direction
        rb.linearVelocity = Vector2.zero; // Stop movement during charge-up
        Debug.Log("Boss is charging up!");

        Invoke(nameof(StartDash), chargeTime); // Begin dash after charge-up
    }

    private void StartDash()
    {
        if (!isCharging) return;

        isCharging = false;
        isDashing = true;

        Vector2 dashTarget = (Vector2)transform.position + dashDirection * dashRange;
        rb.linearVelocity = dashDirection * dashSpeed; // Dash in the locked direction
        Debug.Log($"Boss is dashing towards {dashTarget}!");

        Invoke(nameof(StartSlowDown), dashRange / dashSpeed); // Start slowing down after covering the distance
    }

    private void StartSlowDown()
    {
        if (!isDashing) return;

        isDashing = false;
        isSlowingDown = true;
        StartCoroutine(SlowDownAndStop());
    }

    private System.Collections.IEnumerator SlowDownAndStop()
    {
        float elapsedTime = 0f;
        Vector2 initialVelocity = rb.linearVelocity;

        // Gradually reduce velocity to zero
        while (elapsedTime < slowDownDuration)
        {
            rb.linearVelocity = Vector2.Lerp(initialVelocity, Vector2.zero, elapsedTime / slowDownDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero; // Stop movement
        Debug.Log("Boss has slowed down and stopped.");

        yield return new WaitForSeconds(stopDuration); // Pause for a short duration

        isSlowingDown = false;
        Debug.Log("Boss is ready to resume chasing or charging.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isDashing)
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(1); // Deal damage to the player
                Debug.Log("Player hit by boss dash!");
            }
        }
    }
}
