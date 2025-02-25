using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float speed;         // Speed of the enemy
    public Animator animator;   // Reference to the Animator component

    public int healthPoints;    // HP of the enemy 
    private int maxHealth;       // Store max health for percentage calculations

    private Vector2 movement;   // Store x and y movement

    public float knockbackForce = 5f;
    public int damageToPlayer;

    private GameObject player;  // Reference to the player object (will be assigned dynamically)
    [Tooltip("XP Orb drop")]
    public GameObject xpPrefab;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController normalAnimator;      // Default animator
    public RuntimeAnimatorController halfHealthAnimator;  // Animator for half health
    public RuntimeAnimatorController lowHealthAnimator;   // Animator for low health

    private bool isHalfHealthTriggered = false;
    private bool isLowHealthTriggered = false;

    void Start()
    {
        // Find the player object by its tag when the enemy starts
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No player found!");
        }

        maxHealth = healthPoints; // Store the max health at start
        animator.runtimeAnimatorController = normalAnimator; // Set default animator
    }

    void Update()
    {
        if (player == null) return;  // If no player found, return early

        // Calculate the direction towards the player
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Update the movement values
        movement.x = direction.x;
        movement.y = direction.y;

        // Pass movement values to the animator for animations
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude); // For speed-based animations

        if (movement.x < 0)                                 // Moving left
            transform.localScale = new Vector3(-1, 1, 1);       // Flip the sprite on the x-axis
        else if (movement.x > 0)                            // Moving right
            transform.localScale = new Vector3(1, 1, 1);        // Reset the sprite to face right

        // Move the enemy towards the player
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;  // Reduce enemy's health by damage amount
        Debug.Log($"Hit! Enemy health: {healthPoints}");

        UpdateAnimationState(); // Check if animation should change

        if (healthPoints <= 0)   // Health is zero or less
        {
            Die();
        }
    }

    private void UpdateAnimationState()
    {
        if (healthPoints <= maxHealth * 0.25f && !isLowHealthTriggered)
        {
            animator.runtimeAnimatorController = lowHealthAnimator;
            isLowHealthTriggered = true;
            isHalfHealthTriggered = true; // Ensure we don't switch to half health afterward
        }
        else if (healthPoints <= maxHealth * 0.5f && !isHalfHealthTriggered)
        {
            animator.runtimeAnimatorController = halfHealthAnimator;
            isHalfHealthTriggered = true;
        }
    }

    private void Die()
    {
        if (xpPrefab != null)
        {
            // Instantiate the XP orb at the enemy’s position
            GameObject exp = Instantiate(xpPrefab, transform.position, Quaternion.identity);
        }
        

        // Destroy enemy object
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Check if it collides with the player
        {
            PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damageToPlayer);
                KnockbackPlayer(collision.gameObject);      // Apply knockback
            }
        }
    }

    private void KnockbackPlayer(GameObject player)
    {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Calculate the direction to knock the player back
            Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;

            // Apply knockback force
            playerScript.ApplyKnockback(knockbackDirection * knockbackForce);

            Debug.Log("Knocked back player!!!!");
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        StopAllCoroutines(); // Stop any existing knockback routines
        StartCoroutine(KnockbackRoutine(force));
    }

    private IEnumerator KnockbackRoutine(Vector2 force)
    {
        float knockbackTime = 0.2f;
        float elapsedTime = 0f;

        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + (force.normalized * Mathf.Clamp(force.magnitude, 0.5f, 5f));

        while (elapsedTime < knockbackTime)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / knockbackTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}
