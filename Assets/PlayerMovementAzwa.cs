using UnityEngine;

public class PlayerMovementAzwa : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    private Rigidbody2D rb;
    private Animator animator;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal"); // Get input (-1 for A, 1 for D, 0 for no input)
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); // Apply movement

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
}