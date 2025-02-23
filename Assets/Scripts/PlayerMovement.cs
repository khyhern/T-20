using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Base movement speed
    public float dashSpeed = 15f; // Speed during a dash
    public float dashDuration = 0.2f; // Duration of the dash
    public float dashCooldown = 1f; // Cooldown time between dashes

    public SpriteRenderer characterSprite; // Reference to the SpriteRenderer for flipping

    private Vector2 movement; // Directional movement input
    private Rigidbody2D rb; // Reference to Rigidbody2D

    private bool isDashing = false; // Is the player currently dashing?
    private float dashTime; // Timer to track dash duration
    private float dashCooldownTime; // Timer to track dash cooldown

    private bool isShooting = false; // Is the character currently shooting?
    private Vector2 shootingDirection; // Direction the player is aiming/shooting

    private float speedMultiplier = 1f; // Multiplier for movement speed adjustments
    private bool isKnockedBack = false;

    private bool CanDash => Time.time >= dashCooldownTime;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    void Update()
    {
        if (!isDashing && !isKnockedBack)
        {
            // Get input from the keyboard (WASD or Arrow Keys)
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Normalize movement to maintain consistent speed in all directions
            movement = movement.normalized;

            // Handle dash input
            if (Input.GetKeyDown(KeyCode.Space) && CanDash)
            {
                StartDash();
            }
        }

        // Update dash timer
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                EndDash();
            }
        }

        UpdateCharacterFlip();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            // Move the player at dash speed during a dash
            rb.linearVelocity = movement * dashSpeed;
        }
        else if (!isKnockedBack)
        {
            // Regular movement with speed multiplier
            rb.linearVelocity = movement * moveSpeed * speedMultiplier;
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        dashCooldownTime = Time.time + dashCooldown; // Set cooldown timer
    }

    void EndDash()
    {
        isDashing = false;
    }

    // Adjust the speed multiplier dynamically (e.g., for slowing down while shooting)
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    public void SetShootingDirection(Vector2 direction)
    {
        isShooting = true;
        shootingDirection = direction.normalized;
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    private void UpdateCharacterFlip()
    {
        // Determine the direction the character should face
        bool faceLeft = false;

        if (isShooting)
        {
            // Face the shooting direction
            Debug.DrawLine(transform.position, transform.position + (Vector3)shootingDirection * 2, Color.red);
            faceLeft = shootingDirection.x < 0;
        }
        else if (movement.x != 0)
        {
            // Face the movement direction
            faceLeft = movement.x < 0;
        }

        // Flip the character sprite
        characterSprite.flipX = faceLeft;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction.normalized * force * 1.2f, ForceMode2D.Impulse);
            Invoke(nameof(EndKnockback), 0.2f); // Adjust knockback duration as needed
        }
    }

    private void EndKnockback()
    {
        isKnockedBack = false;
    }
}
