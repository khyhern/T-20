using UnityEngine;

public class PlayerMovementAzwa : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    private Rigidbody2D rb;
    private Animator animator;
    private float moveInput;
    public LayerMask terrainLayer; // Set this to "Terrain" in the Inspector

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate() // Use FixedUpdate for physics-based movement
    {
        moveInput = Input.GetAxisRaw("Horizontal"); // Get input (-1 for A, 1 for D, 0 for no input)

        if (!IsTouchingWall(moveInput)) // Only move if not touching a wall
        {
            Vector2 newPosition = rb.position + new Vector2(moveInput * speed * Time.fixedDeltaTime, 0);
            rb.MovePosition(newPosition); // Moves the player smoothly while respecting walls
        }

        // Flip character based on movement direction
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Update animation state
        if (moveInput != 0)
            animator.Play("Walk"); // Play walk animation
        else
            animator.Play("Idle"); // Play idle animation
    }

    private bool IsTouchingWall(float direction)
    {
        float rayLength = 0.1f; // Small distance in front of the player
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = new Vector2(direction, 0); // Cast in movement direction

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, terrainLayer);

        return hit.collider != null; // Returns true if a wall is detected
    }
}