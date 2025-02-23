using UnityEngine;

public class CameraFollowWithOffset : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public float offsetStrength = 0.5f; // How strongly the camera shifts based on the cursor
    public float smoothSpeed = 5f; // Smoothing factor for camera movement
    public float maxOffsetDistance = 3f; // Maximum offset distance the camera can move

    private Vector3 defaultOffset; // Default offset of the camera from the player's local position

    void Start()
    {
        // Calculate the initial local offset of the camera relative to the player
        defaultOffset = transform.localPosition;
    }

    void LateUpdate()
    {
        Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction and distance of the cursor from the player's position
        Vector3 cursorOffset = cursorWorldPosition - player.position;

        // Adjust the offset strength based on distance
        Vector3 dynamicOffset = cursorOffset * offsetStrength;

        // Ensure the offset doesn't exceed the maximum distance
        dynamicOffset = Vector3.ClampMagnitude(dynamicOffset, maxOffsetDistance);

        // Ensure the camera doesn't shift vertically 
        dynamicOffset.z = 0; // Keep the camera's z-axis consistent

        // Calculate the target local position for the camera
        Vector3 targetLocalPosition = defaultOffset + dynamicOffset;

        // Smoothly move the camera to the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPosition, Time.deltaTime * smoothSpeed);
    }
}
