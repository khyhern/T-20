using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // The target for the camera to follow (usually the player)
    public Transform target;

    // Offset position of the camera relative to the target
    public Vector2 offset;

    // Smooth speed of the camera's movement
    [Range(0, 1)]
    [Tooltip("Speed at which the object follows Player")]
    public float smoothSpeed = 1;

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position using only X and Y
        Vector2 desiredPosition = (Vector2)target.position + offset;

        // Smoothly interpolate between the current position and the desired position
        Vector2 smoothedPosition = Vector2.Lerp((Vector2)transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position, keeping the Z-axis constant
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
