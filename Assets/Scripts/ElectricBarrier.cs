using UnityEngine;

public class ElectricBarrier : MonoBehaviour
{
    public Transform topWall, bottomWall, leftWall, rightWall; // References to the barrier walls
    public Vector2 initialSize = new Vector2(10f, 10f); // Initial size of the barrier
    public Vector2 finalSize = new Vector2(2f, 2f); // Final size of the barrier after shrinking
    public float shrinkDuration = 60f; // Duration of the shrinking effect in seconds
    public float activeDuration = 60f; // Total time the barrier remains active
    public int damageToPlayer = 1; // Damage dealt to the player
    public float knockbackForce = 5f; // Knockback force applied to the player

    private Vector2 currentSize; // Current size of the barrier
    private float elapsedTime = 0f; // Timer for shrinking
    private bool isActive = false; // Tracks if the barrier is active

    private void Start()
    {
        SetBarrierSize(initialSize);
        AssignCollisionHandlers();
    }

    private void Update()
    {
        if (!isActive) return;

        elapsedTime += Time.deltaTime;

        // Gradually shrink the barrier
        if (elapsedTime < shrinkDuration)
        {
            currentSize = Vector2.Lerp(initialSize, finalSize, elapsedTime / shrinkDuration);
            SetBarrierSize(currentSize);
        }

        // Deactivate the barrier after the active duration
        if (elapsedTime >= activeDuration)
        {
            DeactivateBarrier();
        }
    }

    public void ActivateBarrier()
    {
        isActive = true;
        elapsedTime = 0f;
        currentSize = initialSize;
        SetBarrierSize(initialSize);

        // Position the barrier around the player
        if (GameObject.FindGameObjectWithTag("Player") is GameObject player)
        {
            transform.position = player.transform.position;
        }

        gameObject.SetActive(true);
    }

    public void DeactivateBarrier()
    {
        isActive = false;
        gameObject.SetActive(false); // Disable the barrier game object
    }

    private void SetBarrierSize(Vector2 size)
    {
        // Position the walls based on the size
        topWall.localPosition = new Vector3(0, size.y / 2f, 0);
        bottomWall.localPosition = new Vector3(0, -size.y / 2f, 0);
        leftWall.localPosition = new Vector3(-size.x / 2f, 0, 0);
        rightWall.localPosition = new Vector3(size.x / 2f, 0, 0);

        // Scale the walls to fit the size (optional for visuals)
        topWall.localScale = new Vector3(size.x, 1, 1);
        bottomWall.localScale = new Vector3(size.x, 1, 1);
        leftWall.localScale = new Vector3(1, size.y, 1);
        rightWall.localScale = new Vector3(1, size.y, 1);
    }

    private void AssignCollisionHandlers()
    {
        AssignWallHandler(topWall);
        AssignWallHandler(bottomWall);
        AssignWallHandler(leftWall);
        AssignWallHandler(rightWall);
    }

    private void AssignWallHandler(Transform wall)
    {
        if (wall != null)
        {
            WallCollisionHandler handler = wall.gameObject.AddComponent<WallCollisionHandler>();
            handler.Setup(damageToPlayer, knockbackForce, this);
        }
    }
}
