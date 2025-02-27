using UnityEngine;

public class AzwaGunAim : MonoBehaviour
{
    public Transform player; // Reference to the player to check direction
    public Transform gunSprite; // Assign GunSprite to flip it correctly

    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ensure it's 2D

        // Calculate direction to mouse
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate GunPivot toward cursor
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Flip GunSprite when aiming left
        FlipGun(angle);
    }

    void FlipGun(float angle)
    {
        bool playerFacingLeft = player.localScale.x < 0; // Check if player is facing left
        bool shouldFlip = (angle > 90 || angle < -90);

        // Flip GunSprite only, not GunPivot
        if (gunSprite != null)
        {
            gunSprite.localScale = new Vector3(1, shouldFlip ? -1 : 1, 1);
        }

        // Ensure GunPivot's local scale matches player's facing direction
        transform.localScale = new Vector3(playerFacingLeft ? -1 : 1, 1, 1);
    }
}
